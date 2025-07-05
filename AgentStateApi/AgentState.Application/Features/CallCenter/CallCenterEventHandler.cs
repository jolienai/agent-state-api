using AgentState.Application.Repositories;
using AgentState.Application.Shared;
using AgentState.Domain.Constants;
using AgentState.Domain.Entities;
using AgentState.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AgentState.Application.Features.CallCenter;

public class CallCenterEventHandler(
    IValidator<CallCenterEventCommand> validator, 
    ICallCenterRepository agentRepository,
    ILogger<CallCenterEventHandler> logger) : IRequestHandler<CallCenterEventCommand, Result<bool>>
{
    public async Task<Result<bool>> HandleAsync(CallCenterEventCommand request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            if (request.TimestampUtc.IsLessThanOneHour())
                throw new LateEventException(request.TimestampUtc);

            var agent = await agentRepository.GetByIdAsync(request.AgentId, cancellationToken);
            if (agent is null)
                throw new AgentNotFoundException(request.AgentId);
            
            agent.State = SetNewState(request.Action, request.TimestampUtc);
            agent.Skills = SyncAgentSkills(agent.Id, agent.Skills, request.QueueIds!, "system");

            await agentRepository.SaveChangesAsync(cancellationToken); // single save, atomic

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Method} failed, error: {Error}", nameof(CallCenterEventHandler), ex.Message);
            return Result<bool>.Failure(ex);
        }
    }
    
    private static string SetNewState(string action, DateTime timestampUtc)
    {
        switch (action)
        {
            case Activity.CallStarted:
                return Domain.Enums.AgentStateEnum.OnCall.ToString();
            case Activity.StartDoNoDisturb:
            {
                if (timestampUtc.IsLunchTime())
                    return Domain.Enums.AgentStateEnum.OnLunch.ToString();

                // not sure when not lunchtime what should return
                return Domain.Enums.AgentStateEnum.OnCall.ToString();
            }
        }

        // this should never have happened as it is validating the action to be one of the allowed actions
        throw new Exception("Not possible do determine the agent state");
    }
    
    private static List<AgentSkill> SyncAgentSkills(string agentId, List<AgentSkill> skills, List<string> queueIds, string createdBy)
    {
        var distinctQueueIds = new HashSet<string>(queueIds);
        var existingQueueIds = skills.Select(s => s.QueueId).ToHashSet();

        // Remove old skills
        skills.RemoveAll(s => !distinctQueueIds.Contains(s.QueueId));

        // Add new skills
        var newQueueIds = distinctQueueIds.Except(existingQueueIds);

        return newQueueIds.Select(queueId =>
            new AgentSkill
            {
                Id = Guid.NewGuid().ToString(),
                AgentId = agentId,
                QueueId = queueId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            }
        ).ToList();
    }
}