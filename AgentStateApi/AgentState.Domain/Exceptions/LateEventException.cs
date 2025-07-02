namespace AgentState.Domain.Exceptions;

public class LateEventException(DateTime timestamp) : Exception($"The event is too old. Timestamp: {timestamp:O}");