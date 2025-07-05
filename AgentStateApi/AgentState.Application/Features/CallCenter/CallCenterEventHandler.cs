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

            var newState = SetNewState(request.Action, request.TimestampUtc);
            agent.State = newState.ToString();

            SyncAgentSkills(agent, request.QueueIds!, "system");

            await agentRepository.SaveChangesAsync(cancellationToken); // single save, atomic

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Method} failed, error: {Error}", nameof(CallCenterEventHandler), ex.Message);
            return Result<bool>.Failure(ex);
        }
    }
    
    private static Domain.Enums.AgentStateEnum SetNewState(string action, DateTime timestampUtc)
    {
        switch (action)
        {
            case Activity.CallStarted:
                return Domain.Enums.AgentStateEnum.OnCall;
            case Activity.StartDoNoDisturb:
            {
                if (timestampUtc.IsLunchTime())
                    return Domain.Enums.AgentStateEnum.OnLunch;

                // not sure when not lunchtime what should return
                return Domain.Enums.AgentStateEnum.OnCall;
            }
        }

        // this should never have happened as it is validating the action to be one of the allowed actions
        throw new Exception("Not possible do determine the agent state");
    }
    
    private static void SyncAgentSkills(Agent agent, List<string> queueIds, string createdBy)
    {
        var distinctQueueIds = new HashSet<string>(queueIds);
        var existingQueueIds = agent.Skills.Select(s => s.QueueId).ToHashSet();

        // Remove old skills
        agent.Skills.RemoveAll(s => !distinctQueueIds.Contains(s.QueueId));

        // Add new skills
        var newQueueIds = distinctQueueIds.Except(existingQueueIds);
        foreach (var queueId in newQueueIds)
        {
            agent.Skills.Add(new AgentSkill
            {
                Id = Guid.NewGuid().ToString(),
                AgentId = agent.Id,
                QueueId = queueId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            });
        }
    }
}