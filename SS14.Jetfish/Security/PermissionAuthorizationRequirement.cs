using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public sealed class PermissionAuthorizationRequirement : IAuthorizationRequirement
{
    public List<Permission> Permissions { get; }

    public PermissionAuthorizationRequirement(List<Permission> permissions)
    {
        Permissions = permissions;
    }
}