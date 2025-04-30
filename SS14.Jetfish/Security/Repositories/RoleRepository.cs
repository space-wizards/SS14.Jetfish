using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Repositories;

public class RoleRepository : BaseRepository<Role, Guid>
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<Role, Exception>> AddOrUpdate(Role record)
    {
        _context.Entry(record).State = record.Id != Guid.Empty ?
            EntityState.Modified : EntityState.Added;

        return await SaveChanges(record, _context);
    }

    public override async Task<Result<Role, Exception>> Delete(Role record)
    {
        _context.Role.Remove(record);
        return await SaveChanges(record, _context);
    }

    public async Task Delete(IEnumerable<Role> roles)
    {
        _context.Role.RemoveRange(roles);
        await _context.SaveChangesAsync();
    }

    public override async Task<Role?> GetAsync(Guid id)
    {
        return await _context.Role.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> CountAllGlobal()
    {
        return await _context.Role
            .AsNoTracking()
            .Where(x => (x.TeamId == null && x.Policies.Count == 0) || x.Policies.Any(y => y.ResourceId == null || y.Global))
            .CountAsync();
    }
    
    public async Task<IEnumerable<Role>> GetAllGlobal( int limit = 0, int offset = 0, CancellationToken ct = new())
    {
        var query = _context.Role
            .Include(role => role.Policies)
            .ThenInclude(resourcePolicy => resourcePolicy.AccessPolicy)
            .Where(x => (x.TeamId == null && x.Policies.Count == 0) || x.Policies.Any(y => y.ResourceId == null || y.Global))
            .OrderBy(x => x.Id);
        
        var skipTakeQuery = query.Skip(offset);

        if (limit != 0)
            skipTakeQuery = skipTakeQuery.Take(limit);

        return await skipTakeQuery.ToListAsync(ct);
    }

    public async Task<int> CountAsync(Guid? teamId, CancellationToken ct = new())
    {
        var query = _context.Role.AsNoTracking();

        if (teamId.HasValue)
            query = query.Where(role => role.TeamId == teamId.Value);
        
        return await query.CountAsync(ct);
    }
    
    public async Task<IEnumerable<Role>> GetAllAsync(Guid? teamId, int limit = 0, int offset = 0, CancellationToken ct = new())
    {
        var query = _context.Role
            .Include(role => role.Policies)
            .ThenInclude(resourcePolicy => resourcePolicy.AccessPolicy)
            .AsQueryable();

        if (teamId.HasValue)
            query = query.Where(role => role.TeamId == teamId.Value);

        query = query.OrderBy(role => role.Id);
        
        var skipTakeQuery = query.Skip(offset);

        if (limit != 0)
            skipTakeQuery = skipTakeQuery.Take(limit);

        return await skipTakeQuery.ToListAsync(ct);
    }
}