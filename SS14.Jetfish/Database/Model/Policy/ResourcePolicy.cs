using Microsoft.EntityFrameworkCore;

namespace SS14.Jetfish.Database.Model.Policy;

[Owned]
public class ResourcePolicy
{
    public AccessPolicy AccessPolicy { get; set; } = null!;
    public Guid ResourceId { get; set; }
}