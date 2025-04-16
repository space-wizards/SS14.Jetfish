using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Security.Model;

public sealed class Team : IEntityTypeConfiguration<Team>, IResource, IRecord<Guid>
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    [MaxLength(300)]
    public string Name { get; set; } = null!;
    
    public ICollection<TeamMember> TeamMembers { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasMany(p => p.TeamMembers);
    }
}