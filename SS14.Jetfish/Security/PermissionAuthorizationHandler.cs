using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Services.Interfaces;

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

        #endregion

        Guid? resourceId = context.Resource switch
        {
            Guid guid => guid,
            IResource resource => resource.Id,
            _ => null
        };

        if (context.User.HasPermissions(requirement.Permissions, global: true)
            || context.User.HasPermissions(requirement.Permissions, resourceId))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail(new AuthorizationFailureReason(this, "User doesn't have access"));
    }
}
