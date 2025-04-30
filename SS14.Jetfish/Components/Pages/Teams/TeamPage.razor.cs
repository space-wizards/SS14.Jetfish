using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SS14.Jetfish.Components.Pages.Teams.Dialogs;
using SS14.Jetfish.Components.Shared;
using SS14.Jetfish.Components.Shared.Dialogs;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Commands;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Model.FormModel;
using SS14.Jetfish.Projects.Repositories;
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
    private ProjectRepository ProjectRepository { get; set; } = null!;
    
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
    
    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationState { get; set; }
    
    private Team? Team { get; set; }

    private bool _initialized;
    
    private MudDataGrid<Project> _teamGrid = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;
        
        Team = await TeamRepository.GetAsync(TeamId);
        _initialized = true;
        StateHasChanged();
    }

    private async Task EditRoles()
    {
        Navigation.NavigateTo($"/teams/{TeamId}/roles");
        await Task.CompletedTask;
    }

    private async Task OnProjectDelete(Project project)
    {
        if (!await BlazorUtility.ConfirmDelete(DialogService, "project"))
            return;
        
        var command = new DeleteProjectCommand(project);
        var commandResult = await CommandService.Run(command);
        if (!commandResult!.Result!.IsSuccess)
        {
            await UiErrorService.HandleUiError(commandResult.Result.Error);
            return;
        }

        await _teamGrid.ReloadServerData();
    }

    private async Task OnProjectEdit(Project project)
    {
        var userId = await AuthenticationState.GetUserId();
        
        var model = new ProjectFormModel
        {
            UserId = userId,
            Team = Team!,
            Name = project.Name,
            BackgroundSpecifier = project.BackgroundSpecifier
        };

        if (project.BackgroundSpecifier == ProjectBackgroundSpecifier.Color)
            model.BackgroundColor = project.Background;

        var parameter = new DialogParameters<EditProjectDialog>()
        {
            { x => x.Model, model },
            { x => x.ProjectId, project.Id }
        };
        
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };

        var dialogResult = await DialogService.ShowAsync<EditProjectDialog>("Edit Project", parameter, options);
        var result = await dialogResult.Result;

        if (result == null || result.Canceled || result.Data is not ProjectFormModel modelResult)
            return;
        
        StateHasChanged();
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
    
    private async Task Edit()
    {
        var parameters = new DialogParameters<EditTeamDialog> {
        {
            x => x.Team, Team
        }};

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };

        var dialogResult = await DialogService.ShowAsync<EditTeamDialog>("Edit Team", parameters, options);
        var result = await dialogResult.Result;

        if (result?.Data is not Team team || result.Canceled)
            return;
        
        var command = new CreateOrUpdateTeamCommand(team);
        var commandResult = await CommandService.Run(command);
        if (!commandResult!.Result!.IsSuccess)
        {
            await UiErrorService.HandleUiError(commandResult.Result.Error);
            return;
        }

        Team = team;
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

    private async Task<GridData<Project>> LoadTeams(GridState<Project> arg)
    {
        var userId = await AuthenticationState.GetUserId();
        
        var count = await ProjectRepository.CountByPolicyAndTeam(userId, Team!.Id, Permission.ProjectRead);
        if (count == 0)
            return new GridData<Project>();

        var projects = await ProjectRepository.ListByPolicyAndTeam(
            userId,
            Team!.Id,
            Permission.ProjectRead,
            arg.PageSize,
            arg.Page * arg.PageSize);

        return new GridData<Project>
        {
            Items = projects,
            TotalItems = count
        };
    }
}