using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Database.Model.Policy;

namespace SS14.Jetfish.Database.Model;

public class User : IEntityTypeConfiguration<User>
{
    public Guid Id { get; set; }

    [MaxLength(300)]
    public string DisplayName { get; set; } = null!;

    public ICollection<ResourcePolicy> ResourcePolicies { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(p => p.ResourcePolicies);
    }
}