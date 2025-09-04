using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;

namespace SS14.Jetfish.Core.Extensions;

public static class EventBusExtension
{
    public static void AddEventBus(this WebApplicationBuilder builder)
    {
        builder.Services.AddMessagePipe(options =>
        {
            // TODO: We might want to disable this in production
            options.EnableCaptureStackTrace = true;
        });

        builder.Services.AddSingleton<ISynchronizationStateStore, SynchronizationStateStore>();
        builder.Services.AddSingleton<IConcurrentEventBus, ConcurrentEventBus>();
    }
}
