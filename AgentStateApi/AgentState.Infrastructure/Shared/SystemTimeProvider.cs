using AgentState.Application.Shared;

namespace AgentState.Infrastructure.Shared;

public class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}