using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Components.Shared;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Pages.Teams;

public partial class TeamPage : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;
    
    [Inject]
    private TeamRepository TeamRepository { get; set; } = null!;
    
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    
    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;
    
    [Inject]
    private ICommandService CommandService { get; set; } = null!;
    
    [Inject]
    private UiErrorService UiErrorService { get; set; } = null!;
    
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

    private Task OnProjectEdit(Project contextItem)
    {
        return Task.CompletedTask;
    }

    private Task OnProjectShow(Project contextItem)
    {
        Navigation.NavigateTo($"/projects/{contextItem.Id}");
        return Task.CompletedTask;
    }

    private async Task OnCreateProject()
    {
        // TODO: use different query to get projects the user can see instead of all projects for the team
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
        
        StateHasChanged();
    }

    private async Task Delete()
    {
        if (Team == null || !await BlazorUtility.ConfirmDelete(DialogService, "team"))
            return;
        
        var command = new DeleteTeamCommand(Team);
        var result = await CommandService.Run(command);
        if (!result!.Result!.IsSuccess)
        {
            await UiErrorService.HandleUiError(result.Result.Error);
            return;
        }
        
        Snackbar.Add("Team Removed!", Severity.Success);
        Navigation.NavigateTo("/teams");
    }
}