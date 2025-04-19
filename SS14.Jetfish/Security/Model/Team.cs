using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Security.Model;

public sealed class Team : IEntityTypeConfiguration<Team>, IResource, IRecord<Guid>
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    [MaxLength(300)]
    public string Name { get; set; } = null!;

    public ICollection<TeamMember> TeamMembers { get; set; } = null!;

    /// <summary>
    /// The projects the team is assigned to.
    /// </summary>
    /// <remarks>
    /// When assigning projects to roles only allow projects that the team is assigned to<br/>
    /// When removing projects from teams remove the resource policy for that project from all roles of this team. <see cref="Roles"/> 
    /// </remarks>
    public ICollection<Project> Projects { get; set; } = null!;
    
    /// <summary>
    /// Team roles. Members can have one of the roles assigned from this list
    /// </summary>
    /// <remarks>
    /// Don't allow removing a role that is still in use by team members ඞ
    /// </remarks>
    public ICollection<Role> Roles { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasMany(p => p.Roles)
            .WithOne().HasForeignKey(x => x.TeamId);
        
        builder.HasMany(p => p.TeamMembers);
        builder.HasMany(p => p.Projects)
            .WithMany();
        
        builder.ConfigureRowVersionGuid();
    }
}