using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SS14.Jetfish.Security.Model;

[PrimaryKey("TeamId", "UserId")]
public class TeamMember : IEntityTypeConfiguration<TeamMember>
{
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
    }
}