using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Repositories;

public class ProjectRepository : IResourceRepository<Project, Guid>
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<Project, Exception>> AddOrUpdate(Project record)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(Guid id, [NotNullWhen(true)] out Project? result)
    {
        throw new NotImplementedException();
    }

    public Task<Project?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Project>> ListByPolicy(Guid userId, Permission policy)
    {
        var teamQuery = _context.Project.Where(project =>
            _context.TeamMember.Include(member => member.Role)
                .Any(member => member.User.Id == userId
                               && member.Role.Policies.Any(resourcePolicy =>
                                   resourcePolicy.ResourceId == project.Id
                                   && resourcePolicy.AccessPolicy.Permissions.Contains(policy))));

        var query = _context.Project.Where(project =>
                _context.User.Any(user =>
                    user.Id == userId
                    && user.ResourcePolicies.Any(resourcePolicy =>
                        resourcePolicy.ResourceId == project.Id
                        && resourcePolicy.AccessPolicy.Permissions.Contains(policy))))
            .Union(teamQuery);

        return await query.ToListAsync();
    }
}