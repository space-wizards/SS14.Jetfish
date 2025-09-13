using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using SS14.ConfigProvider;
using SS14.Jetfish;
using SS14.Jetfish.Components;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting;
using SS14.Jetfish.FileHosting.Services.GifConversion;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.UserSettings;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

var env = builder.Environment;
builder.Configuration.AddYamlFile("appsettings.yml", false, true);
builder.Configuration.AddYamlFile($"appsettings.{env.EnvironmentName}.yml", true, true);
builder.Configuration.AddYamlFile("appsettings.Secret.yml", true, true);

builder.Services.Configure<FileConfiguration>(builder.Configuration.GetSection(FileConfiguration.Name));
builder.Services.Configure<FFmpegConfiguration>(builder.Configuration.GetSection(FFmpegConfiguration.Name));
builder.Services.Configure<ServerConfiguration>(builder.Configuration.GetSection(ServerConfiguration.Name));
builder.Services.Configure<UserConfiguration>(builder.Configuration.GetSection(UserConfiguration.Name));
builder.Services.Configure<DataSeederConfiguration>(builder.Configuration.GetSection(DataSeederConfiguration.Name));

#endregion

#region Server
var serverConfiguration = new ServerConfiguration();
builder.Configuration.Bind(ServerConfiguration.Name, serverConfiguration);

//Cors
if (serverConfiguration.CorsOrigins != null)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(serverConfiguration.CorsOrigins.ToArray());
            policy.AllowCredentials();
        });
    });
}

//Forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.All;
});

//Logging
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));
builder.Logging.AddSerilog();

//Systemd Support
builder.Host.UseSystemd();


#endregion

#region Database
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("default"), o =>
    {
        o.ConfigureDataSource(s => s.EnableDynamicJson());
    });
    opt.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
});

#endregion

builder.AddOidc();

builder.AddAuthentication();
builder.AddProjects();

builder.AddCommandHandling();
builder.AddEventBus();
builder.AddFileHosting();
builder.AddStartupCheck();

builder.Services.AddScoped<UiErrorService>();
builder.Services.AddScoped<ConfigurationStoreService>();
builder.Services.AddScoped<UserSettingsService>();

builder.Services.AddSingleton<GifConversionQueue>();
builder.Services.AddHostedService<GifConversionHostedService>();


var dataSeederConfiguration = new DataSeederConfiguration();
builder.Configuration.Bind(DataSeederConfiguration.Name, dataSeederConfiguration);
if (dataSeederConfiguration.Enabled)
    builder.Services.AddHostedService<DataSeeder>();

UserSettingsService.DiscoverSettings(typeof(User));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(o =>
    {
        o.MaximumReceiveMessageSize = null;
    });

builder.Services.AddMudServices();
builder.Services.AddMudMarkdownServices();

var app = builder.Build();

//Migrate on startup
if (serverConfiguration.EnableMigrations)
    StartupMigrationHelper.Migrate<ApplicationDbContext>(app);

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_DESIGN_TIME")))
{
    // TODO: This is shitcode, fix

    // This method errors the shit out in design time, because it needs the ConfigurationStore relation to exist, but this fails in design time
    // so i simply check if we are in design time, but that is bad and i think the env variable is depricated
    builder.Configuration.AddConfigurationDb<ApplicationDbContext>(b => b.UseNpgsql(builder.Configuration.GetConnectionString("default"), o =>
    {
        o.ConfigureDataSource(s => s.EnableDynamicJson());
    }));
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging(o => o.GetLevel = HttpContextExtension.GetRequestLogLevel);

app.UseAntiforgery();

app.UseFileHosting();

app.UseAuthentication();
app.UsePermissions();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    //.RequireAuthorization()
    .AddInteractiveServerRenderMode();

app.MapAuthEndpoint();

app.MapGet("/test", async (ICommandService commandService) =>
{
    var debugCommand = new DebugCommand
    {
        Message = "Hello World!"
    };

    await commandService.Run(debugCommand);
})
.RequireAuthorization(nameof(Permission.TeamCreate));

// Abort starting the server when the startup check fails
if (app.RunStartupCheck())
    return 1;

await StartupAssetHelper.FirstTimeAssetSetup(app);

app.Run();

return 0;
