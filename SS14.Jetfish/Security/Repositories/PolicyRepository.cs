using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Repositories;

public class PolicyRepository : BaseRepository<Role, Guid>
{
    private readonly ApplicationDbContext _context;

    public PolicyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<Role, Exception>> AddOrUpdate(Role record)
    {
        _context.Entry(record).State = record.Id != Guid.Empty ?
            EntityState.Modified : EntityState.Added;

        return await SaveChanges(record, _context);
    }

    public async Task<Result<Role, Exception>> Delete(Role record)
    {
        _context.Role.Remove(record);

        return await SaveChanges(record, _context);
    }

    public override bool TryGet(Guid id, [NotNullWhen(true)] out Role? result)
    {
        throw new NotImplementedException();
    }

    public override Task<Role?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Role> GetAllGlobal()
    {
        return _context.Role
            .Include(role => role.Policies)
            .ThenInclude(resourcePolicy => resourcePolicy.AccessPolicy)
            .Where(x => x.Policies.Count == 0 || x.Policies.Any(y => y.ResourceId == null || y.Global))
            .ToList();
    }
}