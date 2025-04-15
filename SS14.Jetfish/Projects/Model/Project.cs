using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Security;

namespace SS14.Jetfish.Projects.Model;

public class Project : IEntityTypeConfiguration<Project>, IResource
{
    public Guid Id { get; set; }
    
    public void Configure(EntityTypeBuilder<Project> builder)
    {
    }
}