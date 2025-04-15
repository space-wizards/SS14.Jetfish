using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using SS14.Jetfish;
using SS14.Jetfish.Components;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Model;
using SS14.MaintainerBot.Core.Helpers;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

var env = builder.Environment;
builder.Configuration.AddYamlFile("appsettings.yml", false, true);
builder.Configuration.AddYamlFile($"appsettings.{env.EnvironmentName}.yml", true, true);
builder.Configuration.AddYamlFile("appsettings.Secret.yml", true, true);

#endregion

#region Server
// Server configuration.
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
});

#endregion

builder.SetupOidc();
builder.SetupAuthentication();
builder.SetupCommandHandling();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

var app = builder.Build();

//Migrate on startup
if (serverConfiguration.EnableMigrations)
    StartupMigrationHelper.Migrate<ApplicationDbContext>(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/login", (string? returnUrl, HttpContext context) => Results.Challenge(new AuthenticationProperties
{
    RedirectUri = returnUrl,
}, [OpenIdConnectDefaults.AuthenticationScheme]));

app.MapGet("/logout", async (string? returnUrl, HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect(returnUrl ?? context.Request.PathBase.Add("/"));
});

app.MapGet("/test", async (ICommandService commandService) =>
{
    var debugCommand = new DebugCommand
    {
        Message = "Hello World!"
    };

    await commandService.Run(debugCommand);
})
.RequireAuthorization(nameof(AccessArea.TeamCreate));

app.Run();