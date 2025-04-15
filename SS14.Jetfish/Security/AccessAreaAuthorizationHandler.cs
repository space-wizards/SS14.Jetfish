using Microsoft.AspNetCore.Authorization;
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

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessAreaAuthorizationRequirement requirement)
    {
        using var scope     = _serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Admin claim is for first setup, it has access to everything
        if (!string.IsNullOrEmpty(_serverConfiguration.AdminClaim)
            && context.User.HasClaim(c => c.Type == _serverConfiguration.AdminClaim))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // TODO: FINISH

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}