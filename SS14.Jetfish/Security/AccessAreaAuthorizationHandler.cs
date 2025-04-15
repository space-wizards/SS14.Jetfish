using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;

namespace SS14.Jetfish.Security;

public sealed class AccessAreaAuthorizationHandler : AuthorizationHandler<AccessAreaAuthorizationRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory; // Required as this is a singleton
    private readonly ServerConfiguration _serverConfiguration = new();

    public AccessAreaAuthorizationHandler(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        configuration.Bind(ServerConfiguration.Name, _serverConfiguration);
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessAreaAuthorizationRequirement requirement)
    {
        using var scope           = _serviceScopeFactory.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Admin claim is for first setup, it has access to everything
        if (!string.IsNullOrEmpty(_serverConfiguration.AdminClaim)
            && context.User.HasClaim(c => c.Type == _serverConfiguration.AdminClaim))
        {
            context.Succeed(requirement);
            return;
        }

        Guid? resourceId = null;
        if (context.Resource is IResource resource)
            resourceId = resource.Id;

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
            .Any(policy => policy.ResourceId == resourceId
                    && policy.AccessPolicy.AccessAreas.Contains(requirement.AccessArea)))
        {
            context.Succeed(requirement);
            return;
        }

        var hasTeamAccess = dbContext.TeamMember
            .Include(teamMember => teamMember.Role)
            .ThenInclude(role => role.Policies)
            .ThenInclude(policy => policy.AccessPolicy)
            .Any(teamMember =>
            teamMember.UserId == user.Id
            && teamMember.Role.Policies.Any(policy => policy.AccessPolicy.AccessAreas.Contains(requirement.AccessArea)
            ));

        if (hasTeamAccess)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail(new AuthorizationFailureReason(this, "User doesn't have access"));
        // TODO: FINISH
        /*
         * 1. Check if user is superadmin (DONE)
           2. Check if the user has been granted access directly
           3. Check one of the users team memberships contains the required access policy
           4. Cache the result

            Proposal to make querying for permissions less complicated:
            Create a table that contains an array of allowed actions (AccessAreas) per user and resource (resource = null for resourceless actions)
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