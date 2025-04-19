using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Security;

namespace SS14.Jetfish.Projects.Model;

public sealed class Project : IEntityTypeConfiguration<Project>, IResource, IRecord<Guid>
{
    public const int ProjectNameMaxLength = 64;

    public Guid Id { get; set; }
    public int Version { get; set; }

    [Required]
    [MaxLength(ProjectNameMaxLength)]
    public required string Name { get; set; }

    // TODO: Implement properties

    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ConfigureRowVersionGuid();
    }
}