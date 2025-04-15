using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SS14.Jetfish.Security.Model;

public sealed class Team : IEntityTypeConfiguration<Team>
{
    public Guid Id { get; set; }

    [MaxLength(300)]
    public string Name { get; set; } = null!;
    
    public ICollection<TeamMember> TeamMembers { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasMany(p => p.TeamMembers);
    }
}