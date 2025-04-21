using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
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

    public async Task<(bool success, IReadOnlyCollection<ValidationResult> errors)> ValidateAndSaveAsync(CancellationToken ct = new())
    {
        var errors = ValidateChanges();
        if (errors.Count != 0)
            return (false, errors);

        await SaveChangesAsync(ct);
        return (true, errors);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        if (ValidateChanges().Count > 0)
            throw new ValidationException();
            
        return base.SaveChangesAsync(cancellationToken);
    }

    private List<ValidationResult> ValidateChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        var validationErrors = new List<ValidationResult>();
        foreach (var entry in entries)
        {
            Validator.TryValidateObject(entry.Entity, new ValidationContext(entry.Entity), validationErrors, true);
        }
        
        return validationErrors;
    }
}