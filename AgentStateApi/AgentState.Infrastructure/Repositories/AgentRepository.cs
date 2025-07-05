using AgentState.Application.Repositories;
using AgentState.Domain.Entities;
using AgentState.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AgentState.Infrastructure.Repositories;

public class AgentRepository (AppDbContext context) : IAgentRepository
{
    private IDbContextTransaction? _transaction;
    
    public async Task<Agent?> GetByIdAsync(string agentId, CancellationToken ct)
    {
        return await context.Agents
            .Include(a => a.Skills)
            .FirstOrDefaultAsync(a => a.Id == agentId, ct);
    }
    
    public Task SyncAgentSkillsAsync(Agent agent, List<string> queueIds, CancellationToken ct)
    {
        var distinctQueueIds = new HashSet<string>(queueIds);
        var existingQueueIds = agent.Skills.Select(s => s.QueueId).ToHashSet();

        // Remove old skills
        agent.Skills.RemoveAll(s => !distinctQueueIds.Contains(s.QueueId));

        // Add new skills
        var newQueueIds = distinctQueueIds.Except(existingQueueIds);
        foreach (var queueId in newQueueIds)
        {
            agent.Skills.Add(new AgentSkill
            {
                Id = Guid.NewGuid().ToString(),
                AgentId = agent.Id,
                QueueId = queueId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system" // TODO: replace with actual user context
            });
        }

        return Task.CompletedTask;
    }

    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        _transaction = await context.Database.BeginTransactionAsync(ct);
    }

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
}