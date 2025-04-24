using System.Diagnostics;
using SS14.Jetfish.FileHosting.Repositories;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Services;

public sealed class ResourceService
{
    private readonly ProjectRepository _projectRepository;
    private readonly TeamRepository _teamRepository;
    private readonly FileRepository _fileRepository;
    
    public ResourceService(ProjectRepository projectRepository, TeamRepository teamRepository, FileRepository fileRepository)
    {
        _projectRepository = projectRepository;
        _teamRepository = teamRepository;
        _fileRepository = fileRepository;
    }

    public async Task<IResource?> GetResource(ResourceType? type, Guid? id)
    {
        if (type == null || !id.HasValue)
            return null;
        
        return type switch
        {
            ResourceType.Team => await _teamRepository.GetSimpleAsync(id.Value),
            ResourceType.Project => await _projectRepository.GetAsync(id.Value),
            ResourceType.File => await _fileRepository.GetAsync(id.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public async Task<ICollection<IResource>> GetResources(ICollection<Guid> resourceIds, ICollection<ResourceType> types)
    {
        var resources = new List<IResource>();

        if (types.Contains(ResourceType.Team))
            resources.AddRange(await _teamRepository.GetMultiple(resourceIds));

        if (types.Contains(ResourceType.Project))
            resources.AddRange(await _projectRepository.GetMultiple(resourceIds));

        if (types.Contains(ResourceType.File))
            resources.AddRange(await _fileRepository.GetMultiple(resourceIds));

        return resources;
    }
}