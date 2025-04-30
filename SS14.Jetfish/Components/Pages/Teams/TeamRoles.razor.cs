using Microsoft.AspNetCore.Components;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Pages.Teams;

public partial class TeamRoles : ComponentBase
{
    [Inject]
    private TeamRepository TeamRepository { get; set; } = null!;
    
    [Parameter]
    public Guid TeamId { get; set; }
    
    private Team? Team { get; set; }

    private bool initialized = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;
        
        Team = await TeamRepository.GetAsync(TeamId);
        initialized = true;
        StateHasChanged();
    }
}