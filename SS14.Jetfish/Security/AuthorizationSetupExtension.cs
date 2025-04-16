﻿using Microsoft.AspNetCore.Authorization;

namespace SS14.Jetfish.Security;

public static class AuthorizationSetupExtension
{
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, AccessAreaPolicyProvider>();
        builder.Services.AddSingleton<IAuthorizationHandler, AccessAreaAuthorizationHandler>();
    }
}