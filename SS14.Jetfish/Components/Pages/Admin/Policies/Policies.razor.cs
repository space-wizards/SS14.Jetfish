using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Pages.Admin.Policies;

public partial class Policies : ComponentBase
{
    private MudDataGrid<AccessPolicy> _grid = null!;

    [Inject]
    private PolicyRepository Repository { get; set; } = null!;
 
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    
    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;
    
    [Inject]
    private UiErrorService UiErrorService { get; set; } = null!;
    
    private async Task<GridData<AccessPolicy>> LoadData(GridState<AccessPolicy> arg)
    {
        var count = await Repository.CountAsync();
        if (count == 0)
            return new GridData<AccessPolicy>();
            
        var policies = await Repository.GetAllAsync(arg.PageSize, arg.Page * arg.PageSize);

        var gridData = new GridData<AccessPolicy>
        {
            Items = policies,
            TotalItems = count
        };

        return gridData;
    }

    private async Task AddPolicy()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        var dialog = await DialogService.ShowAsync<PolicyDialog>("Add Policy", options);
        var result = await dialog.Result;

        if (result == null || result.Canceled)
            return;

        await SaveChangesUpdate((Role) result.Data!);
    }

    private async Task OnEdit(AccessPolicy policy)
    {
        await Task.CompletedTask;
    }

    private async Task OnDelete(AccessPolicy policy)
    {
        await Task.CompletedTask;
    }
    
    private async Task SaveChangesUpdate(Role role)
    {
        //var command = new CreateOrUpdateRoleCommand(role);
        //var commandResult = await CommandService.Run(command);
        //await SaveChangesFinal(commandResult);
        await Task.CompletedTask;
    }

    private async Task SaveChangesFinal(ICommand<Result<AccessPolicy, Exception>>? commandResult)
    {
        if (!commandResult!.Result!.IsSuccess)
        {
            await UiErrorService.HandleUiError(commandResult.Result.Error);
            return;
        }

        Snackbar.Add("Changes Saved!", Severity.Success);
        await _grid.ReloadServerData();
    }
}