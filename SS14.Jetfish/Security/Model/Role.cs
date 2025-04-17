using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Security.Model;

public sealed class Role : IEntityTypeConfiguration<Role>, IRecord<Guid>
{
    public const int MaxDisplayNameLength = 30;

    public Guid Id { get; set; }
    public int Version { get; set; }

    // PROJECT_MANAGER
    [MaxLength(300)]
    public string? IdpName { get; set; }

    // PROJECT_MANAGER -> Project Manager; PM
    [MaxLength(MaxDisplayNameLength)]
    public string DisplayName { get; set; } = null!;

    public ICollection<ResourcePolicy> Policies { get; set; } = [];

    public void Configure(EntityTypeBuilder<Role> builder)
    {
    }
}