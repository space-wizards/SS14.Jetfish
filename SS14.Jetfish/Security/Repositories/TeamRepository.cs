using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Repositories;

public sealed class TeamRepository : BaseRepository<Team, Guid>, IResourceRepository<Team, Guid>
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<Team, Exception>> AddOrUpdate(Team record)
    {
        _context.Entry(record).State = record.Id != Guid.Empty ?
            EntityState.Modified : EntityState.Added;

        return await SaveChanges(record, _context);
    }

    public override bool TryGet(Guid id, [NotNullWhen(true)] out Team? result)
    {
        throw new NotImplementedException();
    }

    public override async Task<Team?> GetAsync(Guid id)
    {
        return await _context.Team.SingleOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<Result<Team, Exception>> Delete(Team record)
    {
        _context.Team.Remove(record);
        return await SaveChanges(record, _context);
    }

    public async Task<int> CountByPolicy(Guid userId, Permission policy)
    {
        return await ListByPolicyQuery(userId, policy).CountAsync();
    }

    /// <summary>
    /// Returns all teams a user has access to.
    /// </summary>
    public async Task<ICollection<Team>> ListByMembership(Guid userId, Permission policy)
    {
        return await ListByPolicyQuery(userId, policy).ToListAsync();
    }

    public async Task<ICollection<Team>> ListByPolicy(Guid userId, Permission policy, int? limit = null, int? offset = null)
    {
        var query = ListByPolicyQuery(userId, policy).OrderBy(x => x.Id);

        IQueryable<Team>? skipTakeQuery = null;
        if (offset.HasValue)
            skipTakeQuery = query.Skip(offset.Value);

        if (limit.HasValue)
            skipTakeQuery = skipTakeQuery != null ? skipTakeQuery.Take(limit.Value) : query.Take(limit.Value);

        return skipTakeQuery != null ? await skipTakeQuery.ToListAsync() : await query.ToListAsync();
    }

    private IQueryable<Team> ListByPolicyQuery(Guid userId, Permission policy)
    {
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
            .Where(user => user.Id == userId)
            .Where(user => user.ResourcePolicies.Count > 0)
            .Any(user => user.ResourcePolicies
                .Where(resourcePolicy => resourcePolicy.ResourceId == team.Id || resourcePolicy.Global)
                .Any(resourcePolicy => resourcePolicy.AccessPolicy.Permissions.Contains(policy))));

        return query.Union(teamQuery);
    }
}