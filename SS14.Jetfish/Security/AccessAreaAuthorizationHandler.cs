using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;

namespace SS14.Jetfish.Security;

public sealed class AccessAreaAuthorizationHandler : AuthorizationHandler<AccessAreaAuthorizationRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory; // Required as this is a singleton

    public AccessAreaAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessAreaAuthorizationRequirement requirement)
    {
        using var scope     = _serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // TODO: FINISH

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}