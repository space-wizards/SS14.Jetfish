using System.ComponentModel;
using SS14.Jetfish.Core.Types;

namespace SS14.Jetfish.Tests.Types;

public class FileSizeTest
{
    [Theory]
    [InlineData("", 0)]
    [InlineData("ABCDEFG", 0)]
    [InlineData("15", 15)]
    [InlineData("15B", 15)]
    [InlineData("10MB", 10 * 1024 * 1024)]
    [InlineData("128MB", 128 * 1024 * 1024)]
    public void ParseString(string str, long bytes)
    {
        var result = TypeDescriptor.GetConverter(typeof(FileSize)).ConvertFromInvariantString(str);
        Assert.IsType<FileSize>(result);
        Assert.Equal(bytes, ((FileSize)result).Bytes);
    }
}