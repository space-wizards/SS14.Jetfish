using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SS14.Jetfish.Components.Pages.Teams.Dialogs;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Model.FormModel;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Pages.Teams;

public partial class Teams : ComponentBase
{
    private MudDataGrid<Team> _dataGrid = null!;

    [Inject]
    public TeamRepository TeamRepository { get; set; } = null!;
    
    [Inject]
    public ICommandService CommandService { get; set; } = null!;
    
    [Inject]
    public IDialogService DialogService { get; set; } = null!;
    
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;
    
    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;
    
    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationState { get; set; }

    private async Task<GridData<Team>> LoadData(GridState<Team> state)
    {
        if (AuthenticationState == null)
            throw new InvalidOperationException("AuthenticationState is null");
        
        var auth = await AuthenticationState;
        var userId = auth.User.Claims.GetUserId();
        if (!userId.HasValue)
            return new GridData<Team>();
        
        var teams = await TeamRepository.ListByPolicy(
            userId.Value, 
            Permission.TeamRead,
            state.PageSize,
            state.Page * state.PageSize
            );

        var count = await TeamRepository.CountByPolicy(userId.Value, Permission.TeamRead);
        
        return new GridData<Team>()
        {
            Items = teams,
            TotalItems = count
        };
    }

    private async Task OnDelete(Team team)
    {
        if (!await BlazorUtility.ConfirmDelete(DialogService, "team"))
            return;
        
        var command = new DeleteTeamCommand(team);
        await CommandService.Run(command);
        await _dataGrid.ReloadServerData();
    }

    private void OnRowClick(DataGridRowClickEventArgs<Team> arg)
    {
        NavigationManager.NavigateTo($"/teams/{arg.Item.Id}");
    }
    
    private async Task CreateTeam()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        var dialog = await DialogService.ShowAsync<NewTeamDialog>("Add Team", options);
        var result = await dialog.Result;

        if (result == null || result.Canceled || result.Data is not NewTeamFormModel model)
            return;

        if (model is { AddSelf: true, ManagerRoleName: not null })
            await SetModelUserId(model);
        
        var command = new CreateTeamCommand(model);
        var commandResult = await CommandService.Run(command);
        
        if (!commandResult!.Result!.IsSuccess)
        {
            await BlazorUtility.DisplayModifiedPopup(DialogService, NavigationManager);
        }

        await _dataGrid.ReloadServerData();
        Snackbar.Add("Team Added!", Severity.Success);
    }

    private async Task SetModelUserId(NewTeamFormModel model)
    {
        if (AuthenticationState == null)
        {
            model.AddSelf = false;
            return;
        }
        
        var auth = await AuthenticationState;
        var userId = auth.User.Claims.GetUserId();
        if (!userId.HasValue)
        {
            model.AddSelf = false;
            return;
        }
        
        model.UserId = userId.Value;
    }
}