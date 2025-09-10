using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SS14.Jetfish.Configuration;

namespace SS14.Jetfish.FileHosting.Converters;

public class GifToVideoConverter : IUploadConverter
{
    private readonly IOptionsMonitor<FileConfiguration> _config;

    public GifToVideoConverter(IOptionsMonitor<FileConfiguration> config)
    {
        _config = config;
    }

    public string ConverterLabel => "gif_video";

    public async Task<ConversionResult> Convert(string inputPath, string outputPath, CancellationToken ct = new())
    {
        using var image = await Image.LoadAsync(inputPath, ct);
        if (!_config.CurrentValue.GifToVideoConversion || _config.CurrentValue.MinimumGifVideoDimensions.CheckBounds(image.Size))
            return ConversionResult.Skip();

        return ConversionResult.ReplacementQueued();
    }
}
