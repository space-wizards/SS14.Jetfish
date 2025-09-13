using System.Diagnostics;
using Serilog;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.FileHosting.Services;

namespace SS14.Jetfish.Core.Services;

public sealed class StartupCheckService
{
    private readonly ILogger<StartupCheckService> _logger;
    private readonly FileConfiguration _fileConfiguration = new();
    private readonly FFmpegConfiguration _ffmpegConfiguration = new();

    public StartupCheckService(IConfiguration configuration, ILogger<StartupCheckService> logger)
    {
        _logger = logger;
        configuration.Bind(FileConfiguration.Name, _fileConfiguration);
        configuration.Bind(FFmpegConfiguration.Name, _ffmpegConfiguration);
    }

    public bool RunStartupCheck()
    {
        _logger.LogInformation("Running startup check:");

        var directoryCheck = CheckMissingDirectories(_fileConfiguration.CreateMissingDirectories);
        var ffmpegCheck = CheckFFmpeg();

        if (directoryCheck || ffmpegCheck)
        {
            _logger.LogError("Startup check failed");
            return true;
        }

        _logger.LogInformation("Startup check complete");
        return false;
    }

    private bool CheckFFmpeg()
    {
        if (!_fileConfiguration.GifToVideoConversion)
            return false;

        _logger.LogInformation("GIF conversion enabled. GIFs larger than {dimensions} will be converted into videos.",
            _fileConfiguration.MinimumGifVideoDimensions);

        _logger.LogInformation("Checking for FFmpeg installation...");

        var startInfo = new ProcessStartInfo
        {
            FileName = _ffmpegConfiguration.Path,
            Arguments = _ffmpegConfiguration.VersionFlag,
            UseShellExecute = false,
            RedirectStandardOutput = false,
            CreateNoWindow = true,
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        try
        {
            process.Start();
        }
        catch (Exception)
        {
            _logger.LogWarning("FFmpeg is not installed or not configured correctly.");
            return true;
        }

        process.WaitForExit();
        if (process.ExitCode != 0)
            _logger.LogWarning("FFmpeg is not installed or not configured correctly.");

        return process.ExitCode != 0;
    }

    private bool CheckMissingDirectories(bool createMissing)
    {
        var upload = CheckMissingDirectory("user content directory", _fileConfiguration.UserContentDirectory, createMissing);
        var converted =CheckMissingDirectory("converted content directory", _fileConfiguration.ConvertedContentDirectory, createMissing);

        return upload || converted;
    }

    private bool CheckMissingDirectory(string pathName, string path, bool createMissing)
    {
        if (Directory.Exists(path))
            return false;

        if (!createMissing)
        {
            _logger.LogError("- {PathName} is missing at: {Path}", pathName, path);
            return true;
        }

        _logger.LogInformation("- Creating missing {PathName} at {Path}", pathName, path);
        Directory.CreateDirectory(path);
        return false;
    }
}
