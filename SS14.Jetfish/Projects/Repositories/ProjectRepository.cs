using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Repositories;

public class ProjectRepository : BaseRepository<Project, Guid>, IResourceRepository<Project, Guid>
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<Project, Exception>> AddOrUpdate(Project record)
    {
        _context.Entry(record).State = record.Id != Guid.Empty ?
            EntityState.Modified : EntityState.Added;

        return await SaveChanges(record, _context);
    }

    public override async Task<Project?> GetAsync(Guid id)
    {
        return await _context.Project.FindAsync(id);
    }

    public override async Task<Result<Project, Exception>> Delete(Project record)
    {
        await _context.FileUsage.Where(usage => usage.ProjectId == record.Id).ExecuteDeleteAsync();

        _context.Project.Remove(record);
        return await SaveChanges(record, _context);
    }

    public async Task<ICollection<Project>> Search(Guid? teamId, string? search, int? limit = null, int? offset = null, CancellationToken ct = default)
    {
        IQueryable<Project> query;

        // Rider doesn't like this being a ternary when using efcore functions later on
        if (teamId != null)
        {
            query = _context.Team.Where(x => x.Id == teamId).SelectMany(x => x.Projects);
        }
        else
        {
            query = _context.Project.AsQueryable();
        }

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => EF.Functions.ILike(x.Name, $"{search}%"));

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.OrderBy(x => x.Id).ToListAsync(ct);
    }

    public async Task<int> CountByPolicyAndTeam(Guid userId, Guid teamId, Permission policy)
    {
        var teamQuery = PolicyTeamQuery(userId, policy)
            .Where(project => _context.Team
                .Where(team => team.Id == teamId)
                .Any(team => team.Projects.Any(x => x.Id == project.Id)));

        var userQuery = PolicyUserQuery(userId, policy)
            .Where(project => _context.Team
                .Where(team => team.Id == teamId)
                .Any(team => team.Projects.Any(x => x.Id == project.Id)));

        var query = userQuery.Union(teamQuery);
        return await query.CountAsync();
    }

    public async Task<ICollection<Project>> ListByPolicyAndTeam(
        Guid userId,
        Guid teamId,
        Permission policy,
        int? limit = null,
        int? offset = null)
    {
        var teamQuery = PolicyTeamQuery(userId, policy)
            .Where(project => _context.Team
                .Where(team => team.Id == teamId)
                .Any(team => team.Projects.Any(x => x.Id == project.Id)));

        var userQuery = PolicyUserQuery(userId, policy)
            .Where(project => _context.Team
                .Where(team => team.Id == teamId)
                .Any(team => team.Projects.Any(x => x.Id == project.Id)));

        var query = userQuery.Union(teamQuery);

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ICollection<Project>> ListByPolicy(ClaimsPrincipal user, Permission policy, int? limit = null, int? offset = null)
    {
        var userId = user.Claims.GetUserId()!.Value;

        var teamQuery = PolicyTeamQuery(userId, policy);
        var userQuery = PolicyUserQuery(userId, policy);

        var query = userQuery.Union(teamQuery);

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.OrderBy(x => x.Id).ToListAsync();
    }

    private IQueryable<Project> PolicyUserQuery(Guid userId, Permission policy)
    {
        var userQuery = _context.Project.Where(project => _context.User
            .Where(user => user.Id == userId)
            .Any(user => user.ResourcePolicies
                .Where(resourcePolicy => resourcePolicy.ResourceId == project.Id || resourcePolicy.Global)
                .Any(resourcePolicy => resourcePolicy.AccessPolicy.Permissions.Contains(policy))));
        return userQuery;
    }

    private IQueryable<Project> PolicyTeamQuery(Guid userId, Permission policy)
    {
        var teamQuery = _context.Project.Where(project => _context.TeamMember
            .Include(member => member.Role)
            .Where(member => member.User.Id == userId).Any(member => member.Role.Policies
                .Where(resourcePolicy => resourcePolicy.ResourceId == project.Id || resourcePolicy.Global)
                .Any(resourcePolicy => resourcePolicy.AccessPolicy.Permissions.Contains(policy))));
        return teamQuery;
    }

    public async Task<ICollection<Project>> GetMultiple(IEnumerable<Guid> ids)
    {
        return await _context.Project.Where(x => ids.Contains(x.Id)).ToListAsync();
    }
}