using AgentState.Application.Repositories;
using AgentState.Application.Shared;
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

            var newState = AgentStateBusinessLogic.State(request);
            agent.State = newState.ToString();

            await agentRepository.SyncAgentSkillsAsync(agent, request.QueueIds!, cancellationToken);

            await agentRepository.SaveChangesAsync(cancellationToken); // single save, atomic

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Method} failed, error: {Error}", nameof(CallCenterEventHandler), ex.Message);
            return Result<bool>.Failure(ex);
        }
    }
}