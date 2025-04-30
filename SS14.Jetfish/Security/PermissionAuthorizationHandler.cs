using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory; // Required as this is a singleton
    private readonly ServerConfiguration _serverConfiguration = new();

    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        configuration.Bind(ServerConfiguration.Name, _serverConfiguration);
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        using var scope           = _serviceScopeFactory.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        #region Admin Claim

        // Admin claim is for first setup, it has access to everything
        if (!string.IsNullOrEmpty(_serverConfiguration.AdminClaim)
            && context.User.HasClaim(c => c.Type == _serverConfiguration.AdminClaim))
        {
            context.Succeed(requirement);
            return;
        }

        Guid? resourceId = context.Resource switch
        {
            Guid guid => guid,
            IResource resource => resource.Id,
            _ => null
        };

        #endregion


        #region Idp Roles

        var hasIdpAccess = await dbContext.HasIdpAccess(context.User, resourceId, requirement.Permissions.ToArray());

        if (hasIdpAccess)
        {
            context.Succeed(requirement);
            return;
        }

        #endregion

        #region User
        var user = await dbContext.User
            .Include(user => user.ResourcePolicies)
            .ThenInclude(resourcePolicy => resourcePolicy.AccessPolicy)
            .SingleOrDefaultAsync(user => user.Id == context.User.Claims.GetUserId());

        if (user == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "User not found in database (Not logged in?)"));
            return;
        }


        if (user.ResourcePolicies
            .Any(policy => (policy.ResourceId == resourceId || policy.Global)
                    && requirement.Permissions.Intersect(policy.AccessPolicy.Permissions).Any()))
        {
            context.Succeed(requirement);
            return;
        }
        #endregion

        var hasTeamAccess = dbContext.TeamMember
            .Include(teamMember => teamMember.Role)
            .ThenInclude(role => role.Policies)
            .ThenInclude(policy => policy.AccessPolicy)
            .Where(teamMember => teamMember.UserId == user.Id)
            .Any(teamMember => teamMember.Role.Policies
                .Where(policy => policy.ResourceId == resourceId || policy.Global)
                .Any(policy => requirement.Permissions.Intersect(policy.AccessPolicy.Permissions).Any()));

        if (hasTeamAccess)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail(new AuthorizationFailureReason(this, "User doesn't have access"));
        /*
         * 1. Check if user is superadmin (DONE)
           2. Check if the user has been granted access directly
           3. Check one of the users team memberships contains the required access policy
           4. Cache the result

            Proposal to make querying for permissions less complicated:
            Create a table that contains an array of allowed actions (Permissions) per user and resource (resource = null for resourceless actions)
            Said table needs to be kept up to date through triggers on authorization related tables.
            Changes need to be normalized to added and removed actions per resource.
            This is better than the above because changes to permissions occur much less frequent than querying if a user has permissions for something
            Triggers would need to be implemented for the following tables:
            - User_ResourcePolicies
            - TeamMember
            - Role_Policies
            - AccessPolicy
            An alternative to triggers would be a materialized view but refreshing that view would mean it gets rebuild completely every time some permission changes
         */
    }
}