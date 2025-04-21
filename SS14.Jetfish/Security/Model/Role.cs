using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Security.Model;

public sealed class Role : IEntityTypeConfiguration<Role>, IRecord<Guid>, IEquatable<Role?>
{
    public const int MaxDisplayNameLength = 30;
    public const int MaxIdpNameLength = 300;

    public Guid Id { get; set; }
    public int Version { get; set; }

    // PROJECT_MANAGER
    [MaxLength(MaxIdpNameLength)]
    public string? IdpName { get; set; }

    // PROJECT_MANAGER -> Project Manager; PM
    [MaxLength(MaxDisplayNameLength)]
    public string DisplayName { get; set; } = null!;

    public Guid? TeamId { get; set; }
    public ICollection<ResourcePolicy> Policies { get; set; } = [];

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ConfigureRowVersionGuid();
    }

    public bool Equals(Role? other)
    {
        if (other is null) return false;
        return Id.Equals(other.Id) && Version == other.Version;
    }

    public override bool Equals(object? obj)
    {
        return obj is Role role && Equals(role);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCodeS
        return HashCode.Combine(Id, Version);
    }
}