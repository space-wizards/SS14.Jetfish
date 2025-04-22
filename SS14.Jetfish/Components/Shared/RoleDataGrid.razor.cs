using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Components.Pages.Admin.Policies;
using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Shared;

public partial class RoleDataGrid : ComponentBase
{
    private MudDataGrid<Role> _grid = null!;

    [Parameter]
    public Guid? TeamId { get; set; }
    
    [Parameter]
    public bool Global { get; set; }
    
    [Inject]
    private RoleRepository Repository { get; set; } = null!;
    
    [Inject]
    private ICommandService CommandService { get; set; } = null!;
    
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    
    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;
    
    [Inject]
    private UiErrorService UiErrorService { get; set; } = null!;
    
    private async Task<GridData<Role>> LoadData(GridState<Role> arg)
    {
        var roles = Global
            ? await Repository.GetAllGlobal(arg.PageSize, arg.Page * arg.PageSize)
            : await Repository.GetAllAsync(TeamId, arg.PageSize, arg.Page * arg.PageSize);
        
        var count = Global
            ? await Repository.CountAllGlobal()
            : await Repository.CountAsync(TeamId);

        var gridData = new GridData<Role>
        {
            Items = roles,
            TotalItems = count
        };

        return gridData;
    }

    private async Task AddPolicy(Role role)
    {
        var parameters = new DialogParameters<APolicyDialog> {
        { x => x.Role, role },
        { x => x.ResourceId, TeamId}
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        var dialog = await DialogService.ShowAsync<APolicyDialog>("Add Policy", parameters, options);
        var result = await dialog.Result;

        if (result == null || result.Canceled)
            return;

        await SaveChangesUpdate((Role) result.Data!);
    }

    private async Task OnPolicyEdit(ResourcePolicy? policy, Role role)
    {
        if (policy == null)
            return;
        
        var parameters = new DialogParameters<APolicyDialog> {
            {
                x => x.Role, role
            },
            {
                x => x.PolicyId, policy.AccessPolicy.Id
            } };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        var dialog = await DialogService.ShowAsync<APolicyDialog>("Edit Policy", parameters, options);
        var result = await dialog.Result;

        if (result == null || result.Canceled)
            return;

        await SaveChangesUpdate((Role) result.Data!);
    }

    private async Task OnPolicyDelete(ResourcePolicy? policy)
    {
        await Task.CompletedTask;
    }

    private string GetPolicyType(ResourcePolicy policy)
    {
        return policy.Global
            ? "Global"
            : policy.ResourceId.HasValue
                ? "Resource"
                : "General";
    }

    private async Task OnRoleEdit(Role role)
    {
        var parameters = new DialogParameters<RoleDialog> { { x => x.Role, role } };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        var dialog = await DialogService.ShowAsync<RoleDialog>("Edit Role", parameters, options);
        var result = await dialog.Result;

        if (result == null || result.Canceled)
            return;

        await SaveChangesUpdate((Role) result.Data!);
    }

    private async Task OnRoleDelete(Role role)
    {
        if (!await BlazorUtility.ConfirmDelete(DialogService, "role"))
            return;

        var command = new DeleteRoleCommand(role);
        var commandResult = await CommandService.Run(command);
        await SaveChangesFinal(commandResult);
    }

    private async Task CreateRole()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        
        var dialog = await DialogService.ShowAsync<RoleDialog>("Add Role", options);
        var result = await dialog.Result;

        if (result == null || result.Canceled)
            return;

        var role = (Role) result.Data!;
        role.TeamId = TeamId;
        await SaveChangesUpdate((Role) result.Data!);
    }
    
    private async Task SaveChangesUpdate(Role role)
    {
        var command = new CreateOrUpdateRoleCommand(role);
        var commandResult = await CommandService.Run(command);
        await SaveChangesFinal(commandResult);
    }

    private async Task SaveChangesFinal(ICommand<Result<Role, Exception>>? commandResult)
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