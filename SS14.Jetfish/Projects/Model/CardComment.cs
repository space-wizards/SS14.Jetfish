using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Model;

public sealed class CardComment : IEntityTypeConfiguration<CardComment>, IRecord<Guid>
{
    public const int MaxCommentContentLength = 50_000;

    public Guid Id { get; set; }
    public int Version { get; set; }

    public Guid CardId { get; set; }
    public Card Card { get; set; } = null!;

    public User Author { get; set; } = null!;

    [Column(TypeName = "Text")]
    [MaxLength(MaxCommentContentLength)]
    public required string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    public void Configure(EntityTypeBuilder<CardComment> builder)
    {
        builder.ConfigureRowVersionGuid();
        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(MaxCommentContentLength);
    }
}
