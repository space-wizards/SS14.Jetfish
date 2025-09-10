using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;
using SixLabors.ImageSharp;

namespace SS14.Jetfish.Core.Types;

[TypeConverter(typeof(DimensionsTypeConverter))]
public sealed record Dimensions(int Width, int Height)
{

    [PublicAPI]
    public static Dimensions Zero => new(0, 0);

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public static Dimensions Parse(string str)
    {
        var split = str.Split('x');
        if (!int.TryParse(split[0], out var width))
            return Zero;

        if (split.Length < 2 || !int.TryParse(split[1], out var height))
            return new Dimensions(width, width);

        return new Dimensions(width, height);
    }

    public bool CheckBounds(Size size)
    {
        return Width > size.Width || Height > size.Height;
    }
}

public sealed class DimensionsTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string str)
            throw new InvalidOperationException();

        return Dimensions.Parse(str);
    }
}
