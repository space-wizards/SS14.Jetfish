using SS14.Jetfish.Security.Model;
using Xunit.Abstractions;

namespace SS14.Jetfish.Tests.EFCore;

public class PermissionTests
{
    private readonly ITestOutputHelper _output;

    public PermissionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public static IEnumerable<object[]> GetPermissions()
    {
        yield return new object[]
        {
            new Permission[]
            {
                Permission.PoliciesRead,
                Permission.TeamCreate,
                Permission.TeamDelete
            }
        };
        yield return new object[]
        {
            new Permission[]
            {
                Permission.PoliciesRead,
                Permission.PoliciesRead,
                Permission.PoliciesWrite
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetPermissions))]
    public void PermissionsExtensions_GetPolicyNames(Permission[] areas)
    {
        var stringRep = PermissionExtensions.GetPolicyNames(areas);

        if (stringRep.EndsWith(';'))
        {
            Assert.Fail("Should not end with ;");
        }

        _output.WriteLine(stringRep);
    }
}