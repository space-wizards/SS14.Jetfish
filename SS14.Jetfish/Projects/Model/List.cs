using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Projects.Model;

[PrimaryKey(nameof(ProjectId), nameof(ListId))]
public sealed class List : IEntityTypeConfiguration<List>, IRecord<(Guid, int)>
{
    public const int ListNameMaxLength = 128;

    [NotMapped]
    public (Guid, int) Id => (ProjectId, ListId);
    public int Version { get; set; }

    public Guid ProjectId { get; set; }

    public int ListId { get; set; }

    public Project Project { get; set; } = null!;

    public long Position { get; set; }

    [Required]
    [MaxLength(ListNameMaxLength)]
    public string Name { get; set; } = null!;

    public void Configure(EntityTypeBuilder<List> builder)
    {
        builder.Property(x => x.Version).IsRowVersion();
    }
}
