using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.FileHosting.Model;

[Index(nameof(UploadedFileId))]
[PrimaryKey(nameof(CardId), nameof(UploadedFileId))]
public sealed class FileUsage : IEntityTypeConfiguration<FileUsage>
{
    public Guid CardId { get; set; }
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