namespace AgentState.Domain.Exceptions;

public class AgentNotFoundException(string agentId) : Exception($"Agent {agentId} not found.");