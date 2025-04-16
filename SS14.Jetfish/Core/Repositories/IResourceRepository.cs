using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Core.Repositories;

public interface IResourceRepository<T, TKey> : IRepository<T, TKey> where T : class, IResource, IRecord<TKey>
{
    public Task<ICollection<T>> ListByPolicy(Guid userId, AccessArea policy);
}