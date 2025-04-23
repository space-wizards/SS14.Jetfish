using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
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

    public override Task<Project?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public override Task<Result<Project, Exception>> Delete(Project record)
    {
        throw new NotImplementedException();
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
    
    public async Task<ICollection<Project>> ListByPolicy(Guid userId, Permission policy, int? limit = null, int? offset = null)
    {
        var teamQuery = _context.Project.Where(project =>
            _context.TeamMember.Include(member => member.Role)
                .Any(member => member.User.Id == userId
                               && member.Role.Policies.Any(resourcePolicy =>
                                   (resourcePolicy.ResourceId == project.Id || resourcePolicy.Global)
                                   && resourcePolicy.AccessPolicy.Permissions.Contains(policy))));

        var query = _context.Project.Where(project =>
                _context.User.Any(user =>
                    user.Id == userId
                    && user.ResourcePolicies.Any(resourcePolicy =>
                        (resourcePolicy.ResourceId == project.Id || resourcePolicy.Global)
                        && resourcePolicy.AccessPolicy.Permissions.Contains(policy))))
            .Union(teamQuery);

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.OrderBy(x => x.Id).ToListAsync();
    }
}