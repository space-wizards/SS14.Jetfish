﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using SS14.Jetfish.Database.Model.Policy;

namespace SS14.Jetfish.Security;

public sealed class AccessAreaPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly Dictionary<string, AccessArea> _accessAreaPolicies = new Dictionary<string, AccessArea>();

    public AccessAreaPolicyProvider()
    {
        foreach (var accessArea in Enum.GetValues<AccessArea>())
        {
            _accessAreaPolicies.Add(Enum.GetName(accessArea)!, accessArea);
        }
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!_accessAreaPolicies.TryGetValue(policyName, out var policy))
            return Task.FromResult<AuthorizationPolicy?>(null);

        var policyBuilder = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
        policyBuilder.AddRequirements(new AccessAreaAuthorizationRequirement(policy));

        // NULL SUPPRESSION!! (C# expects a nullable type, our return value cant be null because we are making it above)
        return Task.FromResult(policyBuilder.Build())!;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => Task.FromResult(new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        =>  Task.FromResult<AuthorizationPolicy?>(null);
}