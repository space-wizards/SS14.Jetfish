using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Database;

// Contains helper methods

public partial class ApplicationDbContext
{
    /// <summary>
    /// Checks if the given claims principle has global (or scoped) access for any of the permissions given.
    /// </summary>
    /// <param name="user">The user claim</param>
    /// <param name="resourceId">The optional resource to check for.</param>
    /// <param name="permissions">Array of permissions to check for. This is a "OR" check, not "AND" check.</param>
    public async Task<bool> HadIdpAccess(ClaimsPrincipal user, Guid? resourceId, params Permission[] permissions)
    {
        var roles = user.Claims.Where(c => c.Type == _serverConfiguration.RoleClaim)
            .Select(c => c.Value)
            .ToList();

        if (roles.Count <= 0)
            return false;

        var woofArf = permissions.ToList();

        // ඞ
        var hasIdpAccess = Role
            .Include(role => role.Policies)
            .ThenInclude(resourcePolicy => resourcePolicy.AccessPolicy)
            .Where(x => x.IdpName != null && roles.Contains(x.IdpName))
            .Where(x => x.Policies.Any(y => y.ResourceId == resourceId || y.Global))
            .Where(x =>
                x.Policies.Any(y => woofArf.Intersect(y.AccessPolicy.Permissions).Any()));

       return await hasIdpAccess.AnyAsync();
    }
}