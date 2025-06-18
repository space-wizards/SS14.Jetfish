using System.Security.Claims;
using System.Text;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security;

public static class PermissionClaimParserExtension
{
    public const string ClaimPrefix = "perm_";
    public const string GlobalClaimPrefix = "perm_global";

    public static string ClaimName(this PermissionClaim claim)
    {
        var resId = claim.ResourceId.HasValue
            ? claim.ResourceId.Value.ToString()
            : "generic";

        return claim.Global
            ? GlobalClaimPrefix
            : ClaimPrefix + resId;
    }

    public static string ClaimValue(this PermissionClaim claim)
    {
        var value = new StringBuilder();
        value.Append(claim.ResourceType.HasValue ? (short)claim.ResourceType.Value : -1);
        value.Append(';');

        AppendPermissions(claim, value);

        return value.ToString();
    }

    public static string AppendClaimValue(this PermissionClaim claim, string previousValue)
    {
        var value = new StringBuilder(previousValue);

        AppendPermissions(claim, value);

        return value.ToString();
    }

    private static void AppendPermissions(PermissionClaim claim, StringBuilder value)
    {
        foreach (var permission in claim.Permissions)
        {
            value.Append((short)permission);
            value.Append(';');
        }
    }

}
