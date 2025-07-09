namespace AgentState.Application.Shared;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}