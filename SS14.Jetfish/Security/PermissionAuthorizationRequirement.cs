using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public sealed class PermissionAuthorizationRequirement : IAuthorizationRequirement
{
    public Permission[] Permissions { get; }

    public PermissionAuthorizationRequirement(Permission[] permissions)
    {
        Permissions = permissions;
    }
}