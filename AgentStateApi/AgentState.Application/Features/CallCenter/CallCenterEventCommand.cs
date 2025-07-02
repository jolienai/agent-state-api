using AgentState.Application.Shared;

namespace AgentState.Application.Features.CallCenter;

public record  CallCenterEventCommand(
     string AgentId,
     string AgentName,
     DateTime TimestampUtc,
     string Action,
     List<string>? QueueIds) : IRequest<Result<bool>>;