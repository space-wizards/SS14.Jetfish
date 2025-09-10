using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;

namespace SS14.Jetfish.FileHosting.Services.GifConversion;

public class GifConversionHostedService : BackgroundService
{
    private readonly IOptionsMonitor<FFmpegConfiguration> _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly GifConversionQueue _queue;
    private readonly ILogger<GifConversionHostedService> _logger;

    private readonly ApplicationDbContext _context;

    public GifConversionHostedService(
        IOptionsMonitor<FFmpegConfiguration> config,
        IServiceProvider serviceProvider,
        GifConversionQueue queue,
        ILogger<GifConversionHostedService> logger)
    {
        _config = config;
        _serviceProvider = serviceProvider;
        _queue = queue;
        _logger = logger;

        _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    }


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("{ServiceName} started", nameof(GifConversionHostedService));

        while (!ct.IsCancellationRequested)
        {
            var item = await _queue.DequeueAsync(ct);
            // TODO: Implement GIF to video conversion
        }
    }
}
