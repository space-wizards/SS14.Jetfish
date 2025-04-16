using SS14.Jetfish.Security.Model;
using Xunit.Abstractions;

namespace SS14.Jetfish.Tests.EFCore;

public class AccessAreaTests
{
    private readonly ITestOutputHelper _output;

    public AccessAreaTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public static IEnumerable<object[]> GetAccessAreas()
    {
        yield return new object[]
        {
            new AccessArea[]
            {
                AccessArea.AdminReadGlobalPolicies,
                AccessArea.TeamCreate,
                AccessArea.TeamDelete
            }
        };
        yield return new object[]
        {
            new AccessArea[]
            {
                AccessArea.AdminReadGlobalPolicies,
                AccessArea.AdminReadGlobalPolicies,
                AccessArea.AdminWriteGlobalPolicies
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetAccessAreas))]
    public void AccessAreaExtensions_GetPolicyNames(AccessArea[] areas)
    {
        var stringRep = AccessAreaExtensions.GetPolicyNames(areas);

        if (stringRep.EndsWith(';'))
        {
            Assert.Fail("Should not end with ;");
        }

        _output.WriteLine(stringRep);
    }
}