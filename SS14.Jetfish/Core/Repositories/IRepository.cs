using System.Diagnostics.CodeAnalysis;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Core.Repositories;

public interface IRepository <T, TKey> where T : class, IRecord<TKey>
{
    public Task<Result<T, Exception>> AddOrUpdate(T record);
    public Task<T?> GetAsync(TKey id);
    Task<Result<T, Exception>> Delete(T record);
}