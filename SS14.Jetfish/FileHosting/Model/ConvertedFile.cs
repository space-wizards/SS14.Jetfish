using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.FileHosting.Model;

public class ConvertedFile  : IEntityTypeConfiguration<ConvertedFile>, IRecord<Guid>
{
    public Guid Id { get; set; }

    public int Version { get; set; }

    public Guid UploadedFileId { get; set; }

    [MaxLength(260)]
    public required string Label { get; set; }

    [MaxLength(260)]
    public required string RelativePath { get; set; }

    [MaxLength(180)]
    public required string MimeType { get; set; }

    [MaxLength(300)]
    public required string Etag { get; set; }

    public void Configure(EntityTypeBuilder<ConvertedFile> builder)
    {
        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => x.UploadedFileId);
    }
}
