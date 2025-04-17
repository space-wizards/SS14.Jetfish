using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly Dictionary<string, Permission> _permissionPolicies = new Dictionary<string, Permission>();

    public PermissionPolicyProvider()
    {
        foreach (var permission in Enum.GetValues<Permission>())
        {
            _permissionPolicies.Add(Enum.GetName(permission)!, permission);
        }
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policies = policyName.Split(';');
        var areaPolicies = new List<Permission>();
        foreach (var policy in policies)
        {
            if (!_permissionPolicies.TryGetValue(policy, out var parsedPolicy))
                continue;

            areaPolicies.Add(parsedPolicy);
        }

        if (areaPolicies.Count == 0)
            return Task.FromResult<AuthorizationPolicy?>(null);

        var policyBuilder = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
        policyBuilder.AddRequirements(new PermissionAuthorizationRequirement(areaPolicies));

        // NULL SUPPRESSION!! (C# expects a nullable type, our return value cant be null because we are making it above)
        return Task.FromResult(policyBuilder.Build())!;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => Task.FromResult(new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        =>  Task.FromResult<AuthorizationPolicy?>(null);
}