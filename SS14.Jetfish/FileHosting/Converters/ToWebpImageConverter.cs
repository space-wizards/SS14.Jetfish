using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace SS14.Jetfish.FileHosting.Converters;

public class ToWebpImageConverter : IUploadConverter
{
    public string ConverterLabel { get; init; }


    public ToWebpImageConverter(string alternativeLabel)
    {
        ConverterLabel = alternativeLabel;
    }

    public async Task<ConversionResult> Convert(string inputPath, string outputPath, CancellationToken ct = new())
    {
        using var image = await Image.LoadAsync(inputPath, ct);

        if (image.Frames.Count == 0)
            return await InternalConvert(image, outputPath, ct);

        using var frame = image.Frames.ExportFrame(0);
        return await InternalConvert(frame, outputPath, ct);
    }

    private async Task<ConversionResult> InternalConvert(Image image, string outputPath, CancellationToken ct = new())
    {
        var metadata = image.Metadata.GetWebpMetadata();
        metadata.FileFormat = WebpFileFormatType.Lossless;
        await image.SaveAsWebpAsync(Path.Combine(outputPath, Guid.NewGuid() + ".webp"), ct);

        return new ConversionResult(outputPath, "image/webp");
    }
}
