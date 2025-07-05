using AgentState.Application.Repositories;
using AgentState.Application.Shared;
using AgentState.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AgentState.Application.Features.CallCenter;

public class CallCenterEventHandler(
    IValidator<CallCenterEventCommand> validator, 
    IAgentRepository agentRepository,
    ILogger<CallCenterEventHandler> logger) : IRequestHandler<CallCenterEventCommand, Result<bool>>
{
    public async Task<Result<bool>> HandleAsync(CallCenterEventCommand request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. FluentValidation
            await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            // 2. Business rule: event must be < 1 hour old
            if (request.TimestampUtc.IsLessThanOneHour())
                throw new LateEventException(request.TimestampUtc);

            await agentRepository.BeginTransactionAsync(cancellationToken);
            
            // 2. get the agent
            var agent = await agentRepository.GetByIdAsync(request.AgentId, cancellationToken);
            if (agent is null)
                throw new AgentNotFoundException(request.AgentId);
            
            // 3. Calculate state
            var newState = AgentStateBusinessLogic.State(request);

            // 4. Setting the agent’s state based on the calculated value
            agent.State = newState.ToString(); 
            
            // 5.Update the agent’s skills
            await agentRepository.SyncAgentSkillsAsync(agent, request.QueueIds!, cancellationToken);

            await agentRepository.CommitAsync();
               
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Method} failed, error: {Error}", nameof(CallCenterEventHandler), ex.Message);
            await agentRepository.RollbackAsync();
            return Result<bool>.Failure(ex);
        }
    }
}