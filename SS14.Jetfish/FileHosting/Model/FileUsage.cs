using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.FileHosting.Model;

/// <summary>
/// Represents a record that links files to specific usages within a projects context.
/// </summary>
/// <remarks>
/// This class defines the mapping of file usage to specific projects or cards
/// within the application. It includes references to the associated project,
/// card (if applicable), and the uploaded file.
/// </remarks>
[Index(nameof(CardId))]
[Index(nameof(ProjectId))]
[Index(nameof(UploadedFileId))]
public sealed class FileUsage : IEntityTypeConfiguration<FileUsage>, IRecord<Guid>
{
    public Guid Id { get; set; }

    // Entries in this entity only get created or deleted.
    [NotMapped]
    public int Version => 0;
    
    public Guid? CardId { get; set; }
    public Guid UploadedFileId { get; set; }
    public Guid ProjectId { get; set; }

    public Project Project { get; set; } = null!;
    public Card Card { get; set; } = null!;
    
    public void Configure(EntityTypeBuilder<FileUsage> builder)
    {
        builder.HasOne(usage => usage.Project)
            .WithOne()
            .HasForeignKey<FileUsage>(usage => usage.ProjectId);
        
        builder.HasOne(usage => usage.Card)
            .WithOne()
            .HasForeignKey<FileUsage>(usage => usage.CardId);
    }
}