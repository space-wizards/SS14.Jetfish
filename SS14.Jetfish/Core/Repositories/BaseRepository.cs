using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SS14.Jetfish.Core.Types;

namespace SS14.Jetfish.Core.Repositories;

public abstract class BaseRepository<T, TKey> : IRepository<T, TKey> where T : class, IRecord<TKey>
{
    public abstract Task<Result<T, Exception>> AddOrUpdate(T record);

    public abstract  bool TryGet(TKey id, [NotNullWhen(true)] out T? result);

    public abstract Task<T?> GetAsync(TKey id);
    
    protected async Task<Result<T, Exception>> SaveChanges(T record, DbContext context)
    {
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.MessageText != RepositoryConstants.ConcurrencyError)
                throw;

            return Result<T, Exception>.Failure(pgEx);
        }

        return Result<T, Exception>.Success(record);
    }
}