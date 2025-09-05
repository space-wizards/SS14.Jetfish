using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Serilog.Extensions.Logging;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Tests.Types;

namespace SS14.Jetfish.Tests.EventBus;

public sealed class EventBusFixture : IDisposable, IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;
    public IConcurrentEventBus EventBus => _serviceProvider.GetRequiredService<IConcurrentEventBus>();

    public EventBusFixture()
    {
        var services = new ServiceCollection();
        var monitor = new TestOptionsMonitor<ConsoleLoggerOptions>(new ConsoleLoggerOptions());
        services.AddLogging(c => c.AddProvider(new ConsoleLoggerProvider(monitor)));

        services.AddMessagePipe();
        services.AddSingleton<ISynchronizationStateStore, SynchronizationStateStore>();
        services.AddSingleton<IConcurrentEventBus, ConcurrentEventBus>();
        _serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
    }
}
