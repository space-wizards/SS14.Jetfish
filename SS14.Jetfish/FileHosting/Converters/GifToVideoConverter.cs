using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.FileHosting.Services.GifConversion;

namespace SS14.Jetfish.FileHosting.Converters;

public class GifToVideoConverter : IUploadConverter
{
    private readonly IOptionsMonitor<FileConfiguration> _config;
    private readonly GifConversionQueue _queue;

    public GifToVideoConverter(IOptionsMonitor<FileConfiguration> config, GifConversionQueue queue)
    {
        _config = config;
        _queue = queue;
    }

    public string ConverterLabel => "gif_video";

    public async Task<ConversionResult> Convert(Guid fileId, string inputPath, string outputPath, CancellationToken ct = new())
    {
        using var image = await Image.LoadAsync(inputPath, ct);
        if (!_config.CurrentValue.GifToVideoConversion || _config.CurrentValue.MinimumGifVideoDimensions.CheckBounds(image.Size))
            return ConversionResult.Skip();

        var item = new GifConversionItem(fileId, inputPath);
        if(!await _queue.TryQueueItem(item))
            return ConversionResult.Skip();

        return ConversionResult.ReplacementQueued();
    }
}
