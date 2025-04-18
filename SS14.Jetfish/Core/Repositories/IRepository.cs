using System.Diagnostics.CodeAnalysis;
using SS14.Jetfish.Core.Types;

namespace SS14.Jetfish.Core.Repositories;

public interface IRepository <T, TKey> where T : class, IRecord<TKey>
{
    public Result<T, Exception> AddOrUpdate(T record);
    public bool TryGet(TKey id, [NotNullWhen(true)] out T? result);
    public Task<T?> GetAsync(TKey id);
}