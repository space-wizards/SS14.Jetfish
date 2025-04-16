using System.Diagnostics.CodeAnalysis;

namespace SS14.Jetfish.Core.Repositories;

public interface IRepository <T, TKey> where T : IRecord<TKey>
{
    public bool TryAdd(T record, [NotNullWhen(true)] out T? result);
    public bool TryGet(TKey id, [NotNullWhen(true)] out T? result);
    public Task<T?> GetAsync(TKey id);
}