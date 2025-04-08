﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace SS14.Jetfish;

public static class OidcSetupExtension
{
    public static void SetupOidc(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.LoginPath = builder.Configuration["Auth:LoginPath"] ?? "/account/login";
                options.LogoutPath = builder.Configuration["Auth:LogoutPath"] ?? "/account/logout";
                options.ReturnUrlParameter = "returnUrl";
                options.AccessDeniedPath = "/error";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;;

                options.Authority = builder.Configuration["Auth:Authority"];
                options.ClientId = builder.Configuration["Auth:ClientId"];
                options.ClientSecret = builder.Configuration["Auth:ClientSecret"];
                options.MetadataAddress = builder.Configuration["Auth:MetadataAddress"];
                options.SaveTokens = true;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.NameClaimType = "name";

                options.Events.OnTokenValidated = async ctx =>
                {
                    var handler = ctx.HttpContext.RequestServices.GetRequiredService<LoginHandler>();
                    await handler.HandleTokenValidated(ctx);
                };
            });
    }
}