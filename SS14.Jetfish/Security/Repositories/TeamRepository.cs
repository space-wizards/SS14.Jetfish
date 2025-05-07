using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Repositories;

public sealed class TeamRepository : BaseRepository<Team, Guid>, IResourceRepository<Team, Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly ServerConfiguration _serverConfiguration;

    public TeamRepository(ApplicationDbContext context, IOptions<ServerConfiguration> config)
    {
        _context = context;
        _serverConfiguration = config.Value;
    }

    public override async Task<Result<Team, Exception>> AddOrUpdate(Team record)
    {
        _context.Entry(record).State = record.Id != Guid.Empty ?
            EntityState.Modified : EntityState.Added;

        return await SaveChanges(record, _context);
    }

    public async Task<Team?> GetSimpleAsync(Guid id)
    {
        return await _context.Team.SingleOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<Team?> GetAsync(Guid id)
    {
        return await _context.Team
            .AsSplitQuery()
            .Include(t => t.TeamMembers)
            .ThenInclude(t => t.User)
            .Include(t => t.TeamMembers)
            .ThenInclude(t => t.Role)
            .Include(t => t.Roles)
            .Include(t => t.Projects)
            .SingleOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<Result<Team, Exception>> Delete(Team record)
    {
        _context.Team.Remove(record);
        return await SaveChanges(record, _context);
    }

    public async Task<int> CountByPolicy(ClaimsPrincipal user, Permission policy)
    {
        return await (await ListByPolicyQuery(user, policy)).CountAsync();
    }

    /// <summary>
    /// Returns all teams a user has access to.
    /// </summary>
    public async Task<ICollection<Team>> ListByMembership(Guid userId, bool includeProjects = false)
    {
        var query = _context.Team.AsSplitQuery();

        if (includeProjects)
            query = query.Include(t => t.Projects);

        return await query.Include(t => t.TeamMembers)
            .Where(t => t.TeamMembers.Any(tm => tm.UserId == userId))
            .ToListAsync();
    }

    public async Task<ICollection<Team>> ListByPolicy(ClaimsPrincipal user, Permission policy, int? limit = null, int? offset = null)
    {
        var query = (await ListByPolicyQuery(user, policy)).OrderBy(x => x.Id);

        IQueryable<Team>? skipTakeQuery = null;
        if (offset.HasValue)
            skipTakeQuery = query.Skip(offset.Value);

        if (limit.HasValue)
            skipTakeQuery = skipTakeQuery != null ? skipTakeQuery.Take(limit.Value) : query.Take(limit.Value);

        return skipTakeQuery != null ? await skipTakeQuery.ToListAsync() : await query.ToListAsync();
    }

    public async Task<ICollection<Team>> GetMultiple(IEnumerable<Guid> ids)
    {
        return await _context.Team.Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    private async Task<IQueryable<Team>> ListByPolicyQuery(ClaimsPrincipal user, Permission policy)
    {
        var userId = user.Claims.GetUserId();

        var hasGlobalRead = await _context.HasIdpAccess(_serverConfiguration, user, null, Permission.TeamRead);
        if (hasGlobalRead)
        {
            return _context.Team
                .Include(t => t.Projects)
                .Include(t => t.TeamMembers)
                .ThenInclude(tm => tm.User)
                .AsSplitQuery();
        }

        var teamQuery = _context.Team
            .Include(t => t.Projects)
            .Include(t => t.TeamMembers)
            .ThenInclude(tm => tm.User)
            .Where(team => _context.TeamMember
            .Include(member => member.Role)
            .Where(member => member.User.Id == userId)
            .Where(member => member.Role.Policies.Count > 0)
            .Any(member => member.Role.Policies
                .Where(resourcePolicy => resourcePolicy.ResourceId == team.Id || resourcePolicy.Global)
                .Any(resourcePolicy => resourcePolicy.AccessPolicy.Permissions.Contains(policy))));

        var query = _context.Team
            .Include(t => t.Projects)
            .Include(t => t.TeamMembers)
            .ThenInclude(tm => tm.User)
            .Where(team => _context.User
            .Where(u => u.Id == userId)
            .Where(u => u.ResourcePolicies.Count > 0)
            .Any(u => u.ResourcePolicies
                .Where(resourcePolicy => resourcePolicy.ResourceId == team.Id || resourcePolicy.Global)
                .Any(resourcePolicy => resourcePolicy.AccessPolicy.Permissions.Contains(policy))));

        return query.Union(teamQuery);
    }
}
