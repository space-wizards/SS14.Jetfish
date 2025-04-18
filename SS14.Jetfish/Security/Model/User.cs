using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Security.Model;

public sealed class User : IEntityTypeConfiguration<User>, IRecord<Guid>
{
    public Guid Id { get; set; }

    public int Version { get; set; }
    
    [MaxLength(300)]
    public string DisplayName { get; set; } = null!;

    public ICollection<ResourcePolicy> ResourcePolicies { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(p => p.ResourcePolicies);
        builder.ConfigureRowVersion();
    }
}