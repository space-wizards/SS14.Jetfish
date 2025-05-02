using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;

namespace SS14.Jetfish.Database;

/// <summary>
/// Used for migrations to create the DB context.
/// </summary>
[UsedImplicitly]
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddYamlFile("appsettings.yml", true, true)
            .AddYamlFile($"appsettings.{environment}.yml", true, true)
            .AddYamlFile("appsettings.Secret.yml", true, true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(config.GetConnectionString("default"), o =>
        {
            o.ConfigureDataSource(s => s.EnableDynamicJson());
        });

        var serverConfig = config.GetSection(ServerConfiguration.Name).Get<ServerConfiguration>()!;
        var options = Options.Create(serverConfig);

        return new ApplicationDbContext(optionsBuilder.Options, options);
    }
}