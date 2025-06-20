using System.Security.Claims;

namespace SS14.Jetfish.Security.Model;

public class PermissionClaim
{
    public Guid? ResourceId { get; set; }
    public List<Permission> Permissions { get; set; } = [];
    public bool Global;
}
