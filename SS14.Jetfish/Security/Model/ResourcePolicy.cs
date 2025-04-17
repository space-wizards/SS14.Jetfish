using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace SS14.Jetfish.Security.Model;

[Owned]
[PublicAPI]
public sealed class ResourcePolicy
{
    public AccessPolicy AccessPolicy { get; set; } = null!;
    public Guid? ResourceId { get; set; }
    
    /// <summary>
    /// Whether this Policy applies globally to all resources.<br/>
    /// This is used to allow admins global access
    /// </summary>
    public bool Global { get; set; }
}