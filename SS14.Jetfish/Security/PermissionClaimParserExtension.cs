using System.Security.Claims;
using System.Text;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public static class PermissionClaimParserExtension
{
    public const string ClaimPrefix = "perm_";
    public const string GlobalClaimKey = "perm_global";
    public const string AnyClaimKey = "perm_generic";

    public static string ClaimName(this PermissionClaim claim)
    {
        return ClaimName(claim.Global, claim.ResourceId);
    }

    public static string ClaimValue(this PermissionClaim claim, string? previousValue = null)
    {
        var value = new StringBuilder(previousValue);

        AppendPermissions(claim.Permissions, value);

        return value.ToString();
    }

    public static bool HasPermissions(
        this ClaimsPrincipal principal,
        IEnumerable<Permission> permissions,
        Guid? resourceId = null,
        bool global = false)
    {
        var key = ClaimName(global, resourceId);
        var claim = principal.FindFirstValue(key);
        if (claim == null)
            return false;

        var permStrings = permissions.Select(perm => ((short)perm).ToString());
        return permStrings.Any(ps => claim.Contains(ps));
    }

    private static string ClaimName(bool global, Guid? resourceId)
    {
        if (!resourceId.HasValue)
            return AnyClaimKey;

        return global
            ? GlobalClaimKey
            : ClaimPrefix + resourceId.Value;
    }

    public static void AppendPermissions(IEnumerable<Permission> permissions, StringBuilder value)
    {
        foreach (var permission in permissions)
        {
            value.Append((short)permission);
            value.Append(';');
        }
    }



}
