using MudBlazor;
using SS14.Jetfish.FileHosting.Model;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Services.Interfaces;

namespace SS14.Jetfish.Security.Model;

public enum ResourceType : short
{
    Team,
    Project,
    File
}

public static class ResourceTypeExtensions
{
    public static ResourceType GetResourceType(this IResource resource)
    {
        return resource switch
        {
            Team => ResourceType.Team,
            Project => ResourceType.Project,
            UploadedFile => ResourceType.File,
            _ => throw new ArgumentOutOfRangeException(nameof(resource), resource, null)
        };
    }

    public static string GetIcon(this ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Team => Icons.Material.Filled.Group,
            ResourceType.Project => Icons.Material.Filled.Folder,
            ResourceType.File => Icons.Material.Filled.InsertDriveFile,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
        };
    }
}