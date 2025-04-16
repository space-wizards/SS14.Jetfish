using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Security.Model;

[PrimaryKey("TeamId", "UserId")]
public sealed class TeamMember : IEntityTypeConfiguration<TeamMember>, IRecord<(Guid, Guid)>
{
    [NotMapped]
    public (Guid, Guid) Id => (TeamId, UserId);
    public int Version { get; set; }
    
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
    }
}