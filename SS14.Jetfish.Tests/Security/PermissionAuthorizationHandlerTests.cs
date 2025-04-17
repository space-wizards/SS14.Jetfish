using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Tests.Security;

public class PermissionAuthorizationHandlerTests
{
    [Fact]
    public void PermissionAuthorizationHandler_HasAnyPermission()
    {
        Assert.True(PermissionAuthorizationHandler.HasAnyPermission(
            [
                Permission.PoliciesRead,
                Permission.ProjectEdit,
                Permission.ProjectCreate
            ],
            [
                Permission.PoliciesRead
            ]
        ));

        Assert.False(PermissionAuthorizationHandler.HasAnyPermission(
            [
                Permission.PoliciesRead,
                Permission.ProjectEdit,
                Permission.ProjectCreate
            ],
            [
                Permission.TeamCreate
            ]
        ));
    }
}