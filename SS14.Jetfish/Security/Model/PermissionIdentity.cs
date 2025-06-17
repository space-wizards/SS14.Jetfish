namespace SS14.Jetfish.Security.Model;

public class PermissionIdentity
{
    public Guid UserId { get; set; }
    public Guid? ResourceId { get; set; }
    public ResourceType? ResourceType { get; set; }
    public List<Permission> Permissions { get; set; } = [];
    public bool Global;
}
