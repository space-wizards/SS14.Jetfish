using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public class PolicyRepository : IRepository<Role, Guid>
{
    private readonly ApplicationDbContext _context;

    public PolicyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool TryAdd(Role record, [NotNullWhen(true)] out Role? result)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(Guid id, [NotNullWhen(true)] out Role? result)
    {
        throw new NotImplementedException();
    }

    public Task<Role?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Role> GetAllGlobal()
    {
        return _context.Role
            .Include(role => role.Policies)
            .ThenInclude(resourcePolicy => resourcePolicy.AccessPolicy)
            .Where(x => x.IdpName != null)
            .Where(x => x.Policies.Any(y => y.ResourceId == null || y.Global))
            .ToList();
    }
}