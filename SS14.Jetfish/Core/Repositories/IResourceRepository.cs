using System.Security.Claims;
using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Core.Repositories;

public interface IResourceRepository<T, TKey> : IRepository<T, TKey> where T : class, IResource, IRecord<TKey>
{
    public Task<ICollection<T>> ListByPolicy(ClaimsPrincipal user, Permission policy, int? limit = null, int? offset = null);

    public Task<ICollection<T>> GetMultiple(IEnumerable<TKey> ids);
}