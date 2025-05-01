using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;

namespace SS14.Jetfish.Tests.EFCore;

public class MigrationTests
{
    [Fact]
    public void AllModelChangesAreMigrated()
    {
        using var dbContext = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql() // No connection string, if you try to use the DB in here, it will explode.
                .Options,
            new OptionsWrapper<ServerConfiguration>(new ServerConfiguration())
            );

        var services = ((IInfrastructure<IServiceProvider>)dbContext).Instance;
        var modelDiffer = services.GetRequiredService<IMigrationsModelDiffer>();
        var migrationsAssembly = services.GetRequiredService<IMigrationsAssembly>();

        var modelSnapshot = migrationsAssembly.ModelSnapshot?.Model;
        Assert.NotNull(modelSnapshot);

        var hasDiffs = StartupMigrationHelper.DiffsExist(services, migrationsAssembly, modelDiffer);

        if (hasDiffs)
            Assert.Fail($"There are pending model changes not covered by migrations.");
    }
}