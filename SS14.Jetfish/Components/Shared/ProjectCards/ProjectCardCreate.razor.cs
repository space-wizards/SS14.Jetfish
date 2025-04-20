using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Shared.ProjectCards;

public partial class ProjectCardCreate : ComponentBase
{
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Parameter]
    public Team Team { get; set; } = null!;

    [Parameter]
    public EventCallback OnCreate { get; set; }

    private async Task CreateProject()
    {
        var parameters = new DialogParameters<CreateProjectDialog> {
        {
            x => x.Team, Team
        }};

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };

        var dialogResult = await DialogService.ShowAsync<CreateProjectDialog>("Create Project", parameters, options);
        var result = await dialogResult.Result;

        if (result == null || result.Canceled)
            return;

        await OnCreate.InvokeAsync();
    }
}