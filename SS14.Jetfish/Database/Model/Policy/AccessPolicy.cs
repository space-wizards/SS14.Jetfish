using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SS14.Jetfish.Database.Model.Policy;

/// <summary>
/// Controls access policies for various endpoints and actions for Jetfish.
/// </summary>
public sealed class AccessPolicy : IEntityTypeConfiguration<AccessPolicy>
{
    public int Id { get; set; }

    /// <summary>
    /// Areas this policy applies to.
    /// </summary>
    public List<AccessArea> AccessAreas { get; set; } = [];

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