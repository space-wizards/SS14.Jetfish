using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core;
using SS14.Jetfish.FileHosting.Model;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<TeamMember> TeamMember { get; set; }
    public DbSet<AccessPolicy> AccessPolicies { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Project> Project { get; set; }
    public DbSet<UploadedFile> UploadedFile { get; set; }


    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        ValidateChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ValidateChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            if (entity is not IEntityValidator validator)
                continue;

            validator.Validate();
        }
    }
}