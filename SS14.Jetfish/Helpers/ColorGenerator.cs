using System.Globalization;
using Wacton.Unicolour;

namespace SS14.Jetfish.Helpers;

public static class ColorGenerator
{
    public static Unicolour Generate()
    {
        var random = new Random();
        // L = 0.58, C = 0.08
        return new Unicolour(
            ColourSpace.Oklch,
             random.Next(40, 60) * 0.01,
            random.Next(8, 16) * 0.01,
            random.Next(360));
    }

    public static string Hex()
    {
        return Generate().MapToRgbGamut().Hex;
    }

    public static string CssOklch()
    {
        var oklch = Generate().Oklch;
        return string.Create(CultureInfo.InvariantCulture, $"oklch({oklch.L} {oklch.C} {oklch.H})");
    }
}
