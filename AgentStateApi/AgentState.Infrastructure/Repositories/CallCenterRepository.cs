using AgentState.Application.Repositories;
using AgentState.Domain.Entities;
using AgentState.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AgentState.Infrastructure.Repositories;

public class CallCenterRepository(AppDbContext context) : ICallCenterRepository
{
    private IDbContextTransaction? _transaction;

    public async Task<Agent?> GetByIdAsync(string agentId, CancellationToken ct)
    {
        return await context.Agents
            .Include(a => a.Skills)
            .FirstOrDefaultAsync(a => a.Id == agentId, ct);
    }
    
    public async Task BeginTransactionAsync(CancellationToken ct)
        => _transaction = await context.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync()
    {

        try
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction to commit");

            await context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction is null)
        {
            return;
        }

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);

}