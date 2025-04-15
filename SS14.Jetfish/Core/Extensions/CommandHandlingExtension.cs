using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;

namespace SS14.Jetfish.Core.Extensions;

public static class CommandHandlingExtension
{
    public static void SetupCommandHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICommandService, CommandService>();
        builder.Services.AddScoped<ICommandHandler, DebugCommandHandler>();
    }
}