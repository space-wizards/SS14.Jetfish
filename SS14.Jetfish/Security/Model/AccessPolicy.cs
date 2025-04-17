using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Security.Model;

/// <summary>
/// Controls access policies for various endpoints and actions for Jetfish.
/// </summary>
public sealed class AccessPolicy : IEntityTypeConfiguration<AccessPolicy>, IRecord<int>
{
    public int Id { get; set; }
    public int Version { get; set; }

    /// <summary>
    /// Areas this policy applies to.
    /// </summary>
    public List<Permission> Permissions { get; set; } = [];

    /// <summary>
    /// The name assigned to this policy.
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public void Configure(EntityTypeBuilder<AccessPolicy> builder)
    {
        builder.HasKey(x => x.Id);
    }
}