using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Database;

public sealed class DataSeeder(IServiceProvider provider, IConfiguration configuration, ILogger<DataSeeder> logger) : IHostedService
{
    private readonly DataSeederConfiguration _config = new();

    public async Task StartAsync(CancellationToken ct)
    {
        logger.LogInformation("Seeding database:");
        configuration.Bind(DataSeederConfiguration.Name, _config);
        using var scope = provider.CreateScope();
        await PopulatePolicies(scope, ct);
        await PopulateUsers(scope, ct);
    }

    private async Task PopulatePolicies(IServiceScope scope, CancellationToken ct)
    {
        var commandService = scope.ServiceProvider.GetRequiredService<ICommandService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (_config.Policies == null)
            return;

        foreach (var (name, perms) in _config.Policies)
        {
            if (await dbContext.AccessPolicies.AnyAsync(p => p.Name == name, ct))
                continue;

            logger.LogInformation("Creating policy {Name}", name);

            var policy = new AccessPolicy
            {
                Name = name,
                Permissions = perms,
            };

            await commandService.Run(new CreateOrUpdatePolicyCommand(policy));
        }
    }

    private async Task PopulateUsers(IServiceScope scope, CancellationToken ct)
    {
        if (!_config.AdminUserId.HasValue)
            return;

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (await dbContext.User.AnyAsync(u => u.Id == _config.AdminUserId.Value, ct))
            return;

        var policy = await dbContext.AccessPolicies.SingleOrDefaultAsync(p => p.Name == _config.AdminPolicyName, ct);
        if (policy == null)
        {
            logger.LogError("Admin policy {Name} not found", _config.AdminPolicyName);
            return;
        }

        logger.LogInformation("Creating admin user {Name}", _config.AdminUsername);

        var user = new User
        {
            Id = _config.AdminUserId.Value,
            DisplayName = _config.AdminUsername ?? "Admin",
            ProfilePicture = Guid.Empty.ToString(),
            ResourcePolicies =
            [
                new ResourcePolicy
                {
                    AccessPolicyId = policy.Id!.Value,
                    Global = true,
                },
            ],
        };

        await dbContext.User.AddAsync(user, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
