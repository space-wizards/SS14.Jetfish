namespace SS14.Jetfish.Configuration;

public class FFmpegConfiguration
{
    public const string Name = "FFmpeg";

    /// <summary>
    /// The path to the FFmpeg executable used for media processing.
    /// <br/>Can be just the executable name if it's in the PATH.
    /// </summary>
    public string? FFmpegPath { get; set; } = "ffmpeg";

    public string? FFmpegVersionFlag { get; set; } = "-version";
}
