using Serilog;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.FileHosting.Services;

namespace SS14.Jetfish.Core.Services;

public sealed class StartupCheckService
{
    private readonly ILogger<StartupCheckService> _logger;
    private readonly FileConfiguration _fileConfiguration = new();

    public StartupCheckService(IConfiguration configuration, ILogger<StartupCheckService> logger)
    {
        _logger = logger;
        configuration.Bind(FileConfiguration.Name, _fileConfiguration);
    }

    public bool RunStartupCheck()
    {
        _logger.LogInformation("Running startup check:");

        if (CheckMissingDirectories(_fileConfiguration.CreateMissingDirectories))
        {
            _logger.LogError("Startup check failed");
            return true;
        }

        _logger.LogInformation("Startup check complete");
        return false;
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
