using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Serilog;

namespace SS14.Jetfish.Helpers;

//Taken from https://gist.github.com/Tim-Hodge/eea0601a14177c199fe60557eeeff31e
public class StartupMigrationHelper
{
    public StartupMigrationHelper()
    {
        throw new InvalidOperationException("This class should not be constructed."); // workaround to get this class into ILogger<StartupMigrationHelper>
    }


    [PublicAPI]
    public static void Migrate<TContext>(IApplicationBuilder builder) where TContext : DbContext
    {
        Migrate<TContext>(builder.ApplicationServices);
    }

    [PublicAPI]
    public static void Migrate<TContext>(IServiceProvider serviceProvider) where TContext : DbContext
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var ctx   = scope.ServiceProvider.GetRequiredService<TContext>();
        var logger      = scope.ServiceProvider.GetRequiredService<ILogger<StartupMigrationHelper>>();

        var sp = ctx.GetInfrastructure();

        var modelDiffer = sp.GetRequiredService<IMigrationsModelDiffer>();
        var migrationsAssembly = sp.GetRequiredService<IMigrationsAssembly>();

        if (migrationsAssembly.ModelSnapshot == null)
        {
            logger.LogWarning("No model snapshot found. Skipping migrations.");
            return;
        }

        var diffsExist = DiffsExist(sp, migrationsAssembly, modelDiffer);

        if(diffsExist)
        {
            throw new InvalidOperationException("There are differences between the current database model and the most recent migration.");
        }

        ctx.Database.Migrate();
        logger.LogDebug("Applied migrations.");
    }

    public static bool DiffsExist(
        IServiceProvider serviceProvider,
        IMigrationsAssembly migrationsAssembly,
        IMigrationsModelDiffer modelDiffer)
    {
        if (migrationsAssembly.ModelSnapshot == null)
            throw new InvalidOperationException("No model snapshot found.");

        var modelInitializer = serviceProvider.GetRequiredService<IModelRuntimeInitializer>();
        var sourceModel = modelInitializer.Initialize(migrationsAssembly.ModelSnapshot.Model);

        var designTimeModel = serviceProvider.GetRequiredService<IDesignTimeModel>();
        var readOptimizedModel = designTimeModel.Model;

        var diffsExist = modelDiffer.HasDifferences(
            sourceModel.GetRelationalModel(),
            readOptimizedModel.GetRelationalModel());

        return diffsExist;
    }
}