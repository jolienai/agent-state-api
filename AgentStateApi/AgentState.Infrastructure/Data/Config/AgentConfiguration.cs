using AgentState.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentState.Infrastructure.Data.Config;

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("agents");

        builder.HasKey(a => a.Id)
            .HasName("pk_agents");

        builder.Property(a => a.Id)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("id");

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(a => a.State)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasColumnName("state");

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(a => a.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(a => a.UpdatedBy)
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.HasMany(a => a.Skills)
            .WithOne(s => s.Agent)
            .HasForeignKey(s => s.AgentId)
            .HasConstraintName("fk_agents_skills_agent_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}