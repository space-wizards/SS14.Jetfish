using SS14.Jetfish.Core.Types;

namespace SS14.Jetfish.Tests.Types;

public class ResultTests
{
    [Fact]
    public void Success()
    {
        var result = Result<string, string>.Success("Success");
        Assert.Null(result.Error);
        if (!result.TryGetResult(out var successMessage))
            Assert.Fail("Result was not a success.");
        
        Assert.Equal("Success", successMessage);
    }

    [Fact]
    public void Error()
    {
        var result = Result<string, string>.Failure("Error");
        Assert.Null(result.Value);
        Assert.Equal("Error", result.Error);
    }
}