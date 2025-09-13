namespace SS14.Jetfish.Configuration;

public class FFmpegConfiguration
{
    public const string Name = "ffmpeg";

    /// <summary>
    /// The path to the FFmpeg executable used for media processing.
    /// <br/>Can be just the executable name if it's in the PATH.
    /// </summary>
    public string? Path { get; set; } = "ffmpeg";

    public string? VersionFlag { get; set; } = "-version";

    public string ConversionOptionsTemplate { get; set; } = "-y -i \"{0}\" -c:v libvpx-vp9 -crf 12 -b:v 500K  \"{1}\"";
}
