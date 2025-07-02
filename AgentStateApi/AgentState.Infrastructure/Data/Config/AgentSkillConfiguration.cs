using AgentState.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentState.Infrastructure.Data.Config;

public class AgentSkillConfiguration : IEntityTypeConfiguration<AgentSkill>
{
    public void Configure(EntityTypeBuilder<AgentSkill> builder)
    {
        builder.ToTable("agent_skills");

        builder.HasKey(s => s.Id)
            .HasName("pk_agent_skills");

        builder.Property(s => s.Id)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("id");

        builder.Property(s => s.AgentId)
            .IsRequired()
            .HasColumnName("agent_id");

        builder.Property(s => s.QueueId)
            .IsRequired()
            .HasColumnName("queue_id");

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(s => s.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(s => s.UpdatedBy)
            .HasMaxLength(100)
            .HasColumnName("updated_by");
    }
}