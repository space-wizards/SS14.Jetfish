using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Projects.Model;

/// <summary>
/// Represents a lane in the kanban model.
/// </summary>
[PrimaryKey(nameof(ProjectId), nameof(ListId))]
public sealed class Lane : IEntityTypeConfiguration<Lane>, IRecord<(Guid, int)>
{
    public const int ListTitleMaxLength = 128;

    [NotMapped]
    public (Guid, int) Id => (ProjectId, ListId);

    [ConcurrencyCheck]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int Version { get; set; }

    public Guid ProjectId { get; set; }

    public int ListId { get; set; }

    public Project Project { get; set; } = null!;

    [Required]
    [MaxLength(ListTitleMaxLength)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// The order of this list in the project
    /// </summary>
    [Required]
    public float Order { get; set; }

    public IList<Card> Cards { get; set; } = null!;

    public void Configure(EntityTypeBuilder<Lane> builder)
    {
        builder.Property(x => x.Version)
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();

        builder.HasMany<Card>(list => list.Cards)
            .WithOne(c => c.Lane)
            .HasForeignKey(c => new { c.ProjectId, c.ListId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
