using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Database.Model;

namespace SS14.Jetfish.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> User { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}