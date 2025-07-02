namespace AgentState.Domain.Entities;

public abstract class BaseEntity
{
    public string Id { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}