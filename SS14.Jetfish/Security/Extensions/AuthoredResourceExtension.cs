using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Services.Interfaces;

namespace SS14.Jetfish.Security.Extensions;

public static class AuthoredResourceExtension
{
    public static bool IsOwnedBy(this IAuthoredResource resource, User? user)
    {
        return user != null && resource.AuthorId == user.Id;
    }

    public static bool IsOwnedBy(this IAuthoredResource resource, Guid userId)
    {
        return resource.AuthorId == userId;
    }
}
