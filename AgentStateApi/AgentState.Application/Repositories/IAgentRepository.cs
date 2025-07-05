using AgentState.Domain.Entities;

namespace AgentState.Application.Repositories;

public interface IAgentRepository
{
    Task<Agent?> GetByIdAsync(string agentId, CancellationToken ct);
    Task SyncAgentSkillsAsync(Agent agent, List<string> queueIds, CancellationToken ct);
    Task BeginTransactionAsync(CancellationToken ct);
    Task CommitAsync();
    Task RollbackAsync();
}