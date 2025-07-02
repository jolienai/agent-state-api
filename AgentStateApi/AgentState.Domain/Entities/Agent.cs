namespace AgentState.Domain.Entities;

public class Agent : BaseEntity
{
    public string Name { get; set; } = default!;
    public string State { get; set; } = default!;
    public List<AgentSkill> Skills { get; set; } = [];
}