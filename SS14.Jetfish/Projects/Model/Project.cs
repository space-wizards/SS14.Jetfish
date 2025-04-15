using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SS14.Jetfish.Projects.Model;

public class Project : IEntityTypeConfiguration<Project>
{
    public Guid Id { get; set; }
    
    public void Configure(EntityTypeBuilder<Project> builder)
    {
    }
}