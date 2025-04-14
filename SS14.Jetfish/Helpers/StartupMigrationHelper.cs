using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Serilog;

namespace SS14.MaintainerBot.Core.Helpers;

//Taken from https://gist.github.com/Tim-Hodge/eea0601a14177c199fe60557eeeff31e
public static class StartupMigrationHelper
{
    [PublicAPI]
    public static void Migrate<TContext>(IApplicationBuilder builder) where TContext : DbContext
    {
        Migrate<TContext>(builder.ApplicationServices);
    }
    
    [PublicAPI]
    public static void Migrate<TContext>(IServiceProvider serviceProvider) where TContext : DbContext
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<TContext>();

        var sp = ctx.GetInfrastructure();

        var modelDiffer = sp.GetRequiredService<IMigrationsModelDiffer>();
        var migrationsAssembly = sp.GetRequiredService<IMigrationsAssembly>();

        if (migrationsAssembly.ModelSnapshot == null)
        {
            Log.Warning("No model snapshot found. Skipping migrations.");
            return;   
        }
        
        var modelInitializer = sp.GetRequiredService<IModelRuntimeInitializer>();
        var sourceModel = modelInitializer.Initialize(migrationsAssembly.ModelSnapshot.Model);

        var designTimeModel = sp.GetRequiredService<IDesignTimeModel>();
        var readOptimizedModel = designTimeModel.Model;

        var diffsExist = modelDiffer.HasDifferences(
            sourceModel.GetRelationalModel(),
            readOptimizedModel.GetRelationalModel());

        if(diffsExist)
        {
            throw new InvalidOperationException("There are differences between the current database model and the most recent migration.");
        }
        
        ctx.Database.Migrate();
        Log.Debug("Applied migrations.");
    }
}