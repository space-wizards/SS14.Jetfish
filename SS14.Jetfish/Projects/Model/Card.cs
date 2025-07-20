using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Model;

public sealed class Card : IEntityTypeConfiguration<Card>, IRecord<Guid>
{
    public const int CardTitleMaxLength = 200;
    public const int CardDescriptionMaxLength = 50_000;

    public Guid Id { get; set; }
    public int Version { get; set; }

    public User Author { get; set; } = null!;
    public Guid AuthorId { get; set; }

    public Guid ProjectId { get; set; }

    public int ListId { get; set; }

    public Lane Lane { get; set; } = null!;

    [Required]
    [MaxLength(CardTitleMaxLength)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "Text")]
    [MaxLength(CardDescriptionMaxLength)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The order of this list in the project
    /// </summary>
    [Required]
    public float Order { get; set; }

    public Guid? ThumbnailId { get; set; }

    public ICollection<CardComment> Comments { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ConfigureRowVersionGuid();

        builder.HasMany(c => c.Comments)
            .WithOne(cc => cc.Card)
            .HasForeignKey(cc => cc.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
