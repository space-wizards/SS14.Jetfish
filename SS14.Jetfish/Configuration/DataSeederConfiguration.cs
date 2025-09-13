using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Configuration;

public class DataSeederConfiguration
{
    public const string Name = "SeedData";

    public bool Enabled { get; set; } = false;
    public Dictionary<string, List<Permission>>? Policies { get; set; }

    public Guid? AdminUserId { get; set; }
    public string? AdminUsername { get; set; }
    public string? AdminPolicyName { get; set; }
}
