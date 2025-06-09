using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Services.Interfaces;

namespace SS14.Jetfish.FileHosting.Model;

public sealed class UploadedFile : IEntityTypeConfiguration<UploadedFile>, IResource, IRecord<Guid>
{
    public Guid Id { get; set; }

    public int Version { get; set; }

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

    public User? UploadedBy { get; set; } = null!;

    public Guid? UploadedById { get; set; }

    public required ICollection<FileUsage> Usages { get; set; }

    public void Configure(EntityTypeBuilder<UploadedFile> builder)
    {
        builder.HasMany(file => file.Usages)
            .WithOne()
            .HasForeignKey(usage => usage.UploadedFileId);

        builder.HasOne(file => file.UploadedBy)
            .WithMany()
            .HasForeignKey(file => file.UploadedById);

        builder.Property(e => e.LastModified)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}