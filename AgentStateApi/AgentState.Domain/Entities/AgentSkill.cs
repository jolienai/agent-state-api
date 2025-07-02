namespace AgentState.Domain.Entities;

public class AgentSkill : BaseEntity
{
    public string AgentId { get; set; } = default!;
    public Agent Agent { get; set; } = default!;
    public string QueueId { get; set; } = default!;
}