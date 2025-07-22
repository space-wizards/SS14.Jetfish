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

    public bool CreateMissingDirectories { get; set; } = true;
}
