using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;

namespace SS14.Jetfish.FileHosting.Services.GifConversion;

public class GifConversionHostedService : BackgroundService
{
    private readonly IOptionsMonitor<FFmpegConfiguration> _config;
    private readonly IOptionsMonitor<FileConfiguration> _fileConfig;
    private readonly GifConversionQueue _queue;
    private readonly ILogger<GifConversionHostedService> _logger;
    private readonly ApplicationDbContext _context;

    public GifConversionHostedService(
        IOptionsMonitor<FFmpegConfiguration> config,
        IServiceProvider serviceProvider,
        GifConversionQueue queue,
        ILogger<GifConversionHostedService> logger,
        IOptionsMonitor<FileConfiguration> fileConfig)
    {
        var scope = serviceProvider.CreateScope();
        _config = config;
        _queue = queue;
        _logger = logger;
        _fileConfig = fileConfig;
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("{ServiceName} started", nameof(GifConversionHostedService));

        while (!ct.IsCancellationRequested)
        {
            var item = await _queue.DequeueAsync(ct);
            var outputPath = Path.ChangeExtension(item.InputPath, ".webm");
            _logger.LogDebug("Converting {InputPath} to {OutputPath}", item.InputPath, outputPath);

            var startInfo = new ProcessStartInfo
            {
                FileName = _config.CurrentValue.Path,
                Arguments = string.Format(_config.CurrentValue.ConversionOptionsTemplate, item.InputPath, outputPath),
                UseShellExecute = false,
                RedirectStandardOutput = false,
                CreateNoWindow = true,
            };

            using var process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            await process.WaitForExitAsync(ct);

            var uploadedFile = await _context.UploadedFile
                .AsTracking()
                .SingleAsync(x => x.Id == item.FileId, cancellationToken: ct);
            uploadedFile.IsConverting = false;

            if (process.ExitCode != 0)
            {
                _logger.LogError("Conversion of {InputPath} to {OutputPath} failed with exit code {ExitCode}",
                    item.InputPath, outputPath, process.ExitCode);

                await _context.SaveChangesAsync(ct);
                continue;
            }

            uploadedFile.MimeType = "video/webm";
            var relativePath = Path.GetRelativePath(_fileConfig.CurrentValue.UserContentDirectory, outputPath);
            uploadedFile.RelativePath = relativePath;
            await _context.SaveChangesAsync(ct);

            File.Delete(item.InputPath);
        }
    }
}
