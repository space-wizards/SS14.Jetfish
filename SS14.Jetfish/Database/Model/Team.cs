using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Database.Model.Policy;

namespace SS14.Jetfish.Database.Model;

public sealed class Team : IEntityTypeConfiguration<Team>
{
    public Guid Id { get; set; }

    public ICollection<ResourcePolicy> ResourcePolicies { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.OwnsMany(p => p.ResourcePolicies);
    }
}