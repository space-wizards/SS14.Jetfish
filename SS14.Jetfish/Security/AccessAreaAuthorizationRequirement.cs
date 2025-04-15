using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public sealed class AccessAreaAuthorizationRequirement : IAuthorizationRequirement
{
    public AccessArea AccessArea { get; }

    public AccessAreaAuthorizationRequirement(AccessArea accessArea)
    {
        AccessArea = accessArea;
    }
}