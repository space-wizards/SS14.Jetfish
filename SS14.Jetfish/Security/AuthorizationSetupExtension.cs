using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Security.Commands.Handlers;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security;

public static class AuthorizationSetupExtension
{
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        builder.Services.AddScoped<RoleRepository>();
        builder.Services.AddScoped<TeamRepository>();
        builder.Services.AddScoped<UserRepository>();
        builder.Services.AddScoped<TeamMemberRepository>();
        builder.Services.AddScoped<PolicyRepository>();
        builder.Services.AddScoped<ICommandHandler, RoleAddOrRemoveCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, DeleteRoleCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, DeleteTeamCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, CreateTeamCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, CreateOrUpdateTeamCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, ChangeTeamMemberRoleCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, AddTeamMemberCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, DeleteTeamMemberCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, CreateOrUpdatePolicyCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, DeletePolicyCommandHandler>();
    }
}