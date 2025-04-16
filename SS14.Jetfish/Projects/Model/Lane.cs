using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Projects.Model;

[PrimaryKey(nameof(ProjectId), nameof(LaneId))]
public sealed class Lane : IEntityTypeConfiguration<Lane>, IRecord<(Guid, int)>
{
    [NotMapped]
    public (Guid, int) Id => (ProjectId, LaneId);
    public int Version { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public int LaneId { get; set; }
    
    public Project Project { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<Lane> builder)
    {
    }
}