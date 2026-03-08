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
        return await SaveChanges<T>(record, context);
    }

    protected async Task<Result<TR, Exception>> SaveChanges<TR>(TR record, ApplicationDbContext context) where TR : class, IRecord<TKey>
    {
        try
        {
            var result = await context.ValidateAndSaveAsync();
            // TODO: Provide an actual exception message here
            if (!result.success)
                return Result<TR, Exception>.Failure(new ValidationException());
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.MessageText != RepositoryConstants.ConcurrencyError)
                throw;

            return Result<TR, Exception>.Failure(pgEx);
        }
        catch (DbUpdateConcurrencyException concurEx)
        {
            return Result<TR, Exception>.Failure(concurEx);
        }

        return Result<TR, Exception>.Success(record);
    }


}
