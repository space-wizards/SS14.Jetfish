using Microsoft.AspNetCore.Components;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Pages.Teams;

public partial class TeamPage : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;
    
    [Inject]
    private TeamRepository TeamRepository { get; set; } = null!;
    
    [Parameter]
    public Guid TeamId { get; set; }
    
    private Team? Team { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Team = await TeamRepository.GetAsync(TeamId);
    }

    private Task EditRoles()
    {
        return Task.CompletedTask;
    }

    private Task OnProjectDelete(Project contextItem)
    {
        return Task.CompletedTask;
    }
}