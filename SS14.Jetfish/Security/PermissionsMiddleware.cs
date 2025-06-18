using System.Security.Claims;
using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security;

public class PermissionsMiddleware : IMiddleware
{
    private readonly PolicyRepository _repository;
    private readonly IOptions<ServerConfiguration> _serverConfiguration;

    public PermissionsMiddleware(PolicyRepository repository, IOptions<ServerConfiguration> serverConfiguration)
    {
        _repository = repository;
        _serverConfiguration = serverConfiguration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            await next(context);
            return;
        }

        var ct =  context.RequestAborted;
        var userId = context.User.Claims.GetUserId();

        if (!userId.HasValue)
        {
            await context.WriteAccessDeniedResponse("User id not present in token", ct);
            return;
        }

        var roles = context.User.Claims.Where(c => c.Type == _serverConfiguration.Value.RoleClaim)
            .Select(c => c.Value)
            .ToList();

        var permissions = await _repository.GetIdentityPermissions(userId.Value, roles);
        var claims = new Dictionary<string, string>();

        foreach (var permission in permissions)
        {
            var name = permission.ClaimName();
            var value = claims.TryGetValue(name, out var claim)
                ? permission.AppendClaimValue(claim)
                : permission.ClaimValue();

            if (claim != null)
                claims.Remove(name);

            claims.Add(name, value);
        }

        var identity = new ClaimsIdentity(nameof(PermissionsMiddleware), "name", "role");
        identity.AddClaims(claims.Select(c => new Claim(c.Key, c.Value)));

        context.User.AddIdentity(identity);
        await next(context);
    }
}
