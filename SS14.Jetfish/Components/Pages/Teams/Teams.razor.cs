using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;
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
        NavigationManager.NavigateTo($"/team/{arg.Item.Id}");
    }
}