namespace SS14.Jetfish.FileHosting.Converters;

public record ConversionResult(string Path, string MimeType, bool QueuedReplacement = false, bool Skipped = false)
{
    public static ConversionResult Skip() => new(string.Empty, string.Empty, true);
    public static ConversionResult ReplacementQueued() => new(string.Empty, string.Empty, true, true);
}
