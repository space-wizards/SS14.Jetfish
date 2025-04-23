using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Repositories;

public class PolicyRepository : BaseRepository<AccessPolicy, int?>
{
    private readonly ApplicationDbContext _context;

    public PolicyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<AccessPolicy, Exception>> AddOrUpdate(AccessPolicy record)
    {
        _context.Entry(record).State = record.Id != null  ? EntityState.Modified : EntityState.Added;
        return await SaveChanges(record, _context);
    }

    public override async Task<AccessPolicy?> GetAsync(int? id)
    {
        return await _context.AccessPolicies.SingleOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<Result<AccessPolicy, Exception>> Delete(AccessPolicy record)
    {
        _context.AccessPolicies.Remove(record);
        return await SaveChanges(record, _context);
    }
    
    public async Task<int> CountAsync()
    {
        return await _context.AccessPolicies.AsNoTracking().CountAsync();
    }
    
    public async Task<IEnumerable<AccessPolicy>> GetAllAsync(int limit = 0, int offset = 0, CancellationToken ct = new())
    {
        var query = _context.AccessPolicies.OrderBy(role => role.Id).Skip(offset);

        if (limit != 0)
            query = query.Take(limit);

        return await query.ToListAsync(ct);
    }
}