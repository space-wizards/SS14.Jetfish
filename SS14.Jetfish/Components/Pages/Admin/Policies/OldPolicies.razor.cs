using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Pages.Admin.Policies;

public partial class OldPolicies : ComponentBase
{
    private IEnumerable<Role> _roles = [];

    protected override async Task OnInitializedAsync()
    {
        _roles = await RoleRepository.GetAllGlobal();
        StateHasChanged();
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

        await SaveChangesUpdate((Role) result.Data!);
    }

    private bool _editMode = false;

    private void ToggleEdit()
    {
        _editMode = !_editMode;
        StateHasChanged();
    }

    private async Task EditRole(Role role)
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
            await BlazorUtility.DisplayErrorPopup(DialogService, NavigationManager);
        }

        Snackbar.Add("Changes Saved!", Severity.Success);
        _roles = await RoleRepository.GetAllGlobal();
        StateHasChanged();
    }

    private async Task EditPolicy(AccessPolicy policy, Role role)
    {
        if (!_editMode)
            return;

        var parameters = new DialogParameters<APolicyDialog> {
        {
            x => x.Role, role
        },
        {
            x => x.PolicyId, policy.Id
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

    private async Task DeletePolicy(ResourcePolicy policyAccessPolicy, Role context)
    {
        if (!await BlazorUtility.ConfirmDelete(DialogService, "policy"))
            return;

        context.Policies.Remove(policyAccessPolicy);
        await SaveChangesUpdate(context);
    }

    private async Task DeleteRole(Role context)
    {
        if (!await BlazorUtility.ConfirmDelete(DialogService, "role"))
            return;

        var command = new DeleteRoleCommand(context);
        var commandResult = await CommandService.Run(command);
        await SaveChangesFinal(commandResult);
    }

    private async Task AddPolicy(Role context)
    {
        if (!_editMode)
            return;

        var parameters = new DialogParameters<APolicyDialog> {
            {
                x => x.Role, context
            }};

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
}