using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security;

public class PermissionsMiddleware : IMiddleware
{
    private readonly PolicyRepository _repository;
    private readonly IOptions<ServerConfiguration> _serverConfiguration;
    private readonly ILogger<PermissionsMiddleware> _logger;

    public PermissionsMiddleware(PolicyRepository repository, IOptions<ServerConfiguration> serverConfiguration, ILogger<PermissionsMiddleware> logger)
    {
        _repository = repository;
        _serverConfiguration = serverConfiguration;
        _logger = logger;
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

        _logger.LogTrace("Roles for {userId}: {roles}", userId, string.Join(",", roles));

        var permissions = await _repository.GetIdentityPermissions(userId.Value, roles);
        var claims = new Dictionary<string, string>();
        var anyPermissions = new HashSet<Permission>();
        var globalPermissions = new HashSet<Permission>();

        foreach (var permission in permissions)
        {
            foreach (var permissionValue in permission.Permissions)
                anyPermissions.Add(permissionValue);

            if (permission.Global)
                foreach (var permissionValue in permission.Permissions)
                    globalPermissions.Add(permissionValue);

            if (!permission.ResourceId.HasValue)
                continue;

            var name = permission.ClaimName();
            var value = claims.TryGetValue(name, out var claim)
                ? permission.ClaimValue(claim)
                : permission.ClaimValue();

            if (claim != null)
                claims.Remove(name);

            claims.Add(name, value);
        }

        _logger.LogTrace("Permissions for {userId}: {permissions}", userId, string.Join(",", anyPermissions));

        var anyValue = new StringBuilder();
        PermissionClaimParserExtension.AppendPermissions(anyPermissions, anyValue);
        claims.Add(PermissionClaimParserExtension.AnyClaimKey, anyValue.ToString());

        var globalValue = new StringBuilder();
        PermissionClaimParserExtension.AppendPermissions(globalPermissions, globalValue);
        claims.Add(PermissionClaimParserExtension.GlobalClaimKey, globalValue.ToString());

        var identity = new ClaimsIdentity(nameof(PermissionsMiddleware), "name", "role");
        identity.AddClaims(claims.Select(c => new Claim(c.Key, c.Value)));

        context.User.AddIdentity(identity);
        await next(context);
    }
}
