namespace SS14.Jetfish.FileHosting.Converters;

public interface IUploadConverter
{
    public string ConverterLabel { get; }
    public Task<ConversionResult> Convert(Guid fileId, string inputPath, string outputPath, CancellationToken ct = new());
}
