using AgentState.Domain.Entities;

namespace AgentState.Application.Repositories;

public interface ICallCenterRepository
{
    Task<Agent?> GetByIdAsync(string agentId, CancellationToken ct);
    Task BeginTransactionAsync(CancellationToken ct);
    Task CommitAsync();
    Task RollbackAsync();
    Task SaveChangesAsync(CancellationToken cancellationToken);
}