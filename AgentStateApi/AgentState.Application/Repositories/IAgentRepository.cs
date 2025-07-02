using AgentState.Domain.Entities;
using AgentState.Domain.Enums;

namespace AgentState.Application.Repositories;

public interface IAgentRepository
{
    Task<Agent?> GetByIdAsync(string agentId, CancellationToken ct);
    Task UpdateAgentStateAsync(Agent agent, AgentStateEnum newState, CancellationToken ct);
    Task SyncAgentSkillsAsync(Agent agent, List<string> queueIds, CancellationToken ct);
    Task BeginTransactionAsync(CancellationToken ct);
    Task CommitAsync();
    Task RollbackAsync();
}