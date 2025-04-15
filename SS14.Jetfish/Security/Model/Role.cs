using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SS14.Jetfish.Security.Model;

public class Role : IEntityTypeConfiguration<Role>
{
    public Guid Id { get; set; }
    
    [MaxLength(300)]
    public string Name { get; set; } = null!;
    
    public ICollection<ResourcePolicy> Policies { get; set; } = [];

    public void Configure(EntityTypeBuilder<Role> builder)
    {
    }
}