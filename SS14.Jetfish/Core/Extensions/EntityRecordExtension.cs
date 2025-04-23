using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Core.Extensions;

public static class EntityRecordExtension
{
    public static void ConfigureRowVersionGuid<T>(this EntityTypeBuilder<T> builder) where T : class, IRecord<Guid>
    {
        builder.Property(x => x.Version).IsRowVersion();
    }

    public static void ConfigureRowVersionInt<T>(this EntityTypeBuilder<T> builder) where T : class, IRecord<int?>
    {
        builder.Property(x => x.Version).IsRowVersion();
    }
}