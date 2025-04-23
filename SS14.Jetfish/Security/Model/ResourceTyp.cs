using MudBlazor;
using SS14.Jetfish.FileHosting.Model;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Security.Model;

public enum ResourceTyp : short
{
    Team,
    Project,
    File
}

public static class ResourceTypExtensions
{
    public static ResourceTyp GetResourceTyp(this IResource resource)
    {
        return resource switch
        {
            Team => ResourceTyp.Team,
            Project => ResourceTyp.Project,
            UploadedFile => ResourceTyp.File,
            _ => throw new ArgumentOutOfRangeException(nameof(resource), resource, null)
        };
    }

    public static string GetIcon(this ResourceTyp resourceTyp)
    {
        return resourceTyp switch
        {
            ResourceTyp.Team => Icons.Material.Filled.Group,
            ResourceTyp.Project => Icons.Material.Filled.Folder,
            ResourceTyp.File => Icons.Material.Filled.InsertDriveFile,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceTyp), resourceTyp, null)
        };
    }
}