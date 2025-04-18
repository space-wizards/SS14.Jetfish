using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS14.Jetfish.Core.Repositories;

namespace SS14.Jetfish.Core.Extensions;

public static class EntityRecordExtension
{
    public static void ConfigureRowVersion<T>(this EntityTypeBuilder<T> builder) where T : class, IRecord<Guid>
    {
        builder.Property(x => x.Version).IsConcurrencyToken();
            
    }
}