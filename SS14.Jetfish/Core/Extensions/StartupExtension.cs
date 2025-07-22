using SS14.Jetfish.Core.Services;

namespace SS14.Jetfish.Core.Extensions;

public static class StartupExtension
{
    public static void AddStartupCheck(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<StartupCheckService>();
    }

    public static bool RunStartupCheck(this WebApplication app)
    {
        return app.Services.GetRequiredService<StartupCheckService>().RunStartupCheck();
    }
}
