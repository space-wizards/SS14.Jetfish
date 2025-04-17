using System.Text;

namespace SS14.Jetfish.Security.Model;

// When adding new values, ensure that none of the assigned values are changed, DB stores only the numerical value.
public enum Permission : short
{
    // Assign every "area" with its own 10th digit.

    AdminReadGlobalPolicies = 1,
    AdminWriteGlobalPolicies = 2,

    TeamCreate = 10,
    TeamEdit = 11,
    TeamDelete = 12,

    ProjectRead = 20,
}

public static class PermissionExtensions
{
    /**
     * Returns a string containing all given permissions concatenated using a ';' as the delimiter.<br/>
     * This is used to check if any of the given policy applies.
     */
    public static string GetPolicyNames(params Permission[] permissions)
    {
        var returnValue = new StringBuilder();

        for (var index = 0; index < permissions.Length; index++)
        {
            var permission = permissions[index];

            var name = Enum.GetName(permission.GetType(), permission);
            returnValue.Append(name);
            if (index < permissions.Length - 1)
                returnValue.Append(';');
        }

        return returnValue.ToString();
    }

    /**
     * An extension method version of <see cref="GetPolicyNames"/>
     * <inheritdoc cref="GetPolicyNames"/>
     */
    public static string Or(this Permission permission, params Permission[] additionalPermissions)
    {
        additionalPermissions[additionalPermissions.Length] = permission;
        return GetPolicyNames(additionalPermissions);
    }
}