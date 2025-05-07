using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Pages;

[UsedImplicitly]
public partial class Home : ComponentBase
{
    [Inject]
    private TeamRepository TeamRepository { get; set; } = null!;

    [Inject]
    private ProjectRepository ProjectRepository { get; set; } = null!;

    [CascadingParameter]
    public Security.Model.User? User { get; set; }
    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationState { get; set; }


    private ICollection<Team> _teams = [];
    private ICollection<Project> _projects = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await LoadData();
        StateHasChanged();
    }

    private async Task LoadData()
    {
        if (User == null || AuthenticationState == null)
            return;

        var auth = await AuthenticationState;

        _teams = await TeamRepository.ListByMembership(User.Id, true);
        _projects = await ProjectRepository.ListByPolicy(auth.User, Permission.ProjectRead);
    }
}
