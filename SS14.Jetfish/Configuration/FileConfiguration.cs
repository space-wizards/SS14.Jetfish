using SixLabors.ImageSharp;
using SS14.Jetfish.Core.Types;

namespace SS14.Jetfish.Configuration;

public class FileConfiguration
{
    public const string Name = "Files";

    /// <summary>
    /// The directory where user uploaded files are stored.
    /// </summary>
    public string UserContentDirectory { get; set; } = "data/uploads";

    /// <summary>
    /// The maximum upload size for content uploaded by users
    /// </summary>
    public FileSize MaxUploadSize { get; set; } = FileSize.Parse("20MB");

    /// <summary>
    /// The directory where converted files are stored.
    /// </summary>
    public string ConvertedContentDirectory { get; set; } = "data/uploads/converted";

    /// <summary>
    /// Specifies whether missing directories should be automatically created during startup checks.
    /// </summary>
    public bool CreateMissingDirectories { get; set; } = true;

    /// <summary>
    /// Indicates whether GIF files should be automatically converted to mp4 videos.
    /// </summary>
    public bool GifToVideoConversion { get; set; } = false;

    /// <summary>
    /// The minimum dimensions required for converting GIFs to videos.
    /// <br/> GIFs smaller than this will be left as GIFs.
    /// </summary>
    public Dimensions MinimumGifVideoDimensions { get; set; } = Dimensions.Zero;
}
