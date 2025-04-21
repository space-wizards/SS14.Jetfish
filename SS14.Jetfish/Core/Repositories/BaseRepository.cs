using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Core.Repositories;

public abstract class BaseRepository<T, TKey> : IRepository<T, TKey> where T : class, IRecord<TKey>
{
    public abstract Task<Result<T, Exception>> AddOrUpdate(T record);

    public abstract Task<T?> GetAsync(TKey id);
    
    public abstract Task<Result<T, Exception>> Delete(T record);

    protected async Task<Result<T, Exception>> SaveChanges(T record, ApplicationDbContext context)
    {
        try
        {
            var result = await context.ValidateAndSaveAsync();
            if (!result.success)
                return Result<T, Exception>.Failure(new ValidationException());
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.MessageText != RepositoryConstants.ConcurrencyError)
                throw;

            return Result<T, Exception>.Failure(pgEx);
        }
        catch (DbUpdateConcurrencyException concurEx)
        {
            return Result<T, Exception>.Failure(concurEx);
        }

        return Result<T, Exception>.Success(record);
    }
}