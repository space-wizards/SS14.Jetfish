using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace SS14.Jetfish.Security.Model;

[Owned]
[PublicAPI]
public class ResourcePolicy
{
    public AccessPolicy AccessPolicy { get; set; } = null!;
    public Guid? ResourceId { get; set; }
}