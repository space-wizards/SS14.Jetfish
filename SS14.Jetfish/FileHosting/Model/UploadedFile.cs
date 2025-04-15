using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Security;

namespace SS14.Jetfish.FileHosting.Model;

public class UploadedFile : IEntityTypeConfiguration<UploadedFile>, IResource
{
    public Guid Id { get; set; }

    // ඞ
    [MaxLength(260)]
    public required string RelativePath { get; set; }

    [MaxLength(180)]
    public required string MimeType { get; set; }

    [MaxLength(300)]
    public required string Etag { get; set; }

    [MaxLength(300)]
    public required string Name { get; set; }

    public DateTimeOffset LastModified { get; }

    public void Configure(EntityTypeBuilder<UploadedFile> builder)
    {
        builder.Property(e => e.LastModified)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}