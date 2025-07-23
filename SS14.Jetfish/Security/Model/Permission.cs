using System.Text;

namespace SS14.Jetfish.Security.Model;

// When adding new values, ensure that none of the assigned values are changed, DB stores only the numerical value. ඞ
/// <summary>
/// Permissions used for authorizing actions.
/// </summary>
/// <remarks>Resourceless permissions don't apply to any resource and as such use a </remarks>
public enum Permission : short
{
    // Assign every "area" with its own 10th digit.
    /// <remarks>Resourceless permission</remarks>
    PoliciesRead = 1,
    /// <summary>Edit and Delete permission for policies</summary>
    /// <remarks>Resourceless permission</remarks>
    PoliciesWrite = 2,

    /// <remarks>Resourceless permission</remarks>
    TeamCreate = 10,
    TeamRead = 11,
    TeamEdit = 12,
    TeamDelete = 13,

    /// <remarks>Resourceless permission</remarks>
    ProjectCreate = 20,
    ProjectRead = 21,
    ProjectEdit = 22,
    ProjectDelete = 23,

    /// <summary>
    /// Allows to create a card on project. Requires implicit <see cref="ProjectRead"/> to even open the UI.
    /// </summary>
    ProjectCardCreate = 30,
    /// <summary>
    /// Allows one to shuffle around the cards and edit the description, thumbnail etc. on them.
    /// </summary>
    ProjectCardEdit = 31,
    /// <summary>
    /// Allows to delete cards. If not granted, a person who has <see cref="ProjectCardCreate"/> will still be able to delete their own cards.
    /// </summary>
    ProjectCardDelete = 32,
    /// <summary>
    /// Allows a person to comment on a card.
    /// </summary>
    ProjectCardComment = 33,
    /// <summary>
    /// Allows a person to edit comments made by other people.
    /// </summary>
    ProjectCardCommentEdit = 34,
    /// <summary>
    /// Allows deleting comments. Same as deleting cards, a person will always be able to delete their own comments.
    /// </summary>
    ProjectCardCommentDelete = 35,
}

public static class PermissionExtensions
{
    /// <summary>
    /// Returns a string containing all given permissions concatenated using a ';' as the delimiter.<br/>
    /// This is used to check if any of the given policies applies.
    /// </summary>
    /// <param name="permissions">List of permissions to concatenate</param>
    /// <returns>The concatenated list of permissions</returns>
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

   /// <summary>
   /// An extension method version of <see cref="GetPolicyNames"/>
   /// <inheritdoc cref="GetPolicyNames"/>
   /// </summary>
   /// <returns>The concatenated list of permissions</returns>
    public static string Or(this Permission permission, params Permission[] additionalPermissions)
    {
        Array.Resize(ref additionalPermissions, additionalPermissions.Length + 1);;
        additionalPermissions[^1] = permission;
        return GetPolicyNames(additionalPermissions);
    }
}
