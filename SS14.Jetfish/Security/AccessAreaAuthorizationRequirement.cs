using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public sealed class AccessAreaAuthorizationRequirement : IAuthorizationRequirement
{
    public AccessArea[] AccessAreas { get; }

    public AccessAreaAuthorizationRequirement(AccessArea[] accessAreas)
    {
        AccessAreas = accessAreas;
    }
}