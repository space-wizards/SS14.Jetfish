using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
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

    public async Task<IEnumerable<AccessPolicy>> GetAllAsync(string? name = null, bool allPolicies = false, int limit = 0, int offset = 0, CancellationToken ct = new())
    {
        var query = _context.AccessPolicies.AsQueryable();

        if (!allPolicies)
            query = query.Where(policy => policy.TeamAssignable);

        if (name != null)
            query = query.Where(policy =>  EF.Functions.ILike(policy.Name, $"{name}%"));

        var finalQuery = query.OrderBy(role => role.Id).Skip(offset);
        if (limit != 0)
            finalQuery = finalQuery.Take(limit);

        return await finalQuery.ToListAsync(ct);
    }

    public async Task<IEnumerable<PermissionClaim>> GetIdentityPermissions(Guid userId, List<string> idpRoles)
    {
        var userQuery = _context.User.AsNoTracking()
            .Include(u => u.ResourcePolicies)
            .ThenInclude(r => r.AccessPolicy)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.ResourcePolicies.Select(p =>
                new PermissionClaim
                {
                    Global = p.Global,
                    Permissions = p.AccessPolicy.Permissions,
                    ResourceId = p.ResourceId
                }));

        var teamQuery = _context.TeamMember.AsNoTracking()
            .Include(m => m.Role)
            .ThenInclude(r => r.Policies)
            .Where(m => m.UserId == userId)
            .SelectMany(m => m.Role.Policies.Select(p =>
                new PermissionClaim
                {
                    Global = p.Global,
                    Permissions = p.AccessPolicy.Permissions,
                    ResourceId = p.ResourceId
                }));

        if (idpRoles.Count == 0)
            return await userQuery.Union(teamQuery).ToListAsync();

        var idpQuery = _context.Role.AsNoTracking()
            .Include(r => r.Policies)
            .ThenInclude(rp => rp.AccessPolicy)
            .Where(r => r.IdpName != null && idpRoles.Contains(r.IdpName))
            .SelectMany(r => r.Policies.Select(p =>
                new PermissionClaim
                {
                    Global = p.Global,
                    Permissions = p.AccessPolicy.Permissions,
                    ResourceId = p.ResourceId
                }));

        return await userQuery.Union(teamQuery).Union(idpQuery).ToListAsync();
    }
}
