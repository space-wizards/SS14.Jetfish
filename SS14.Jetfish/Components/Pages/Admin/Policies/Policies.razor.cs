using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Commands;
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
    
    [Inject]
    private ICommandService CommandService { get; set; } = null!;
    
    private async Task<GridData<AccessPolicy>> LoadData(GridState<AccessPolicy> arg)
    {
        var count = await Repository.CountAsync();
        if (count == 0)
            return new GridData<AccessPolicy>();
            
        var policies = await Repository.GetAllAsync(limit: arg.PageSize, offset: arg.Page * arg.PageSize);

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

        await SaveChangesUpdate((AccessPolicy) result.Data!);
    }

    private async Task OnEdit(AccessPolicy policy)
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        
        var parameters = new DialogParameters<PolicyDialog> { { x => x.Policy, policy } };
        
        var dialog = await DialogService.ShowAsync<PolicyDialog>("Edit Policy", parameters, options);
        var result = await dialog.Result;

        if (result == null || result.Canceled)
            return;

        await SaveChangesUpdate((AccessPolicy) result.Data!);
    }

    private async Task OnDelete(AccessPolicy policy)
    {
        if (!await BlazorUtility.ConfirmDelete(DialogService, "policy"))
            return;
        
        var command = new DeletePolicyCommand(policy);
        var commandResult = await CommandService.Run(command);
        await SaveChangesFinal(commandResult);
    }
    
    private async Task SaveChangesUpdate(AccessPolicy policy)
    {
        var command = new CreateOrUpdatePolicyCommand(policy);
        var commandResult = await CommandService.Run(command);
        await SaveChangesFinal(commandResult);
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