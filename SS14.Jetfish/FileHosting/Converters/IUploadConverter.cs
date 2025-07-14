namespace SS14.Jetfish.FileHosting.Converters;

public interface IUploadConverter
{
    public string ConverterLabel { get; }
    public Task<ConversionResult> Convert(string inputPath, string outputPath, CancellationToken ct = new());
}

public record ConversionResult(string Path, string MimeType);
