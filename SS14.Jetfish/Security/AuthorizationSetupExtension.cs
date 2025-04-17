using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public static class AuthorizationSetupExtension
{
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        builder.Services.AddScoped<PolicyRepository>();
    }
}