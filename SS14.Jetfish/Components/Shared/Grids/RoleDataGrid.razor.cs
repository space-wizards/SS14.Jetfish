using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Components.Pages.Admin.Policies;
using SS14.Jetfish.Components.Shared.Dialogs;
using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Model.FormModel;
using SS14.Jetfish.Security.Repositories;
using SS14.Jetfish.Security.Services;
using SS14.Jetfish.Security.Services.Interfaces;

namespace SS14.Jetfish.Components.Shared.Grids;

public partial class RoleDataGrid : ComponentBase
{
    private MudDataGrid<Role> _grid = null!;

    private ICollection<IResource> _resources = [];

    [Parameter]
    public Team? Team { get; set; }
    
    [Parameter]
    public bool Global { get; set; }
    
    [Inject]
    private RoleRepository Repository { get; set; } = null!;
    
    [Inject]
    private ProjectRepository ProjectRepository { get; set; } = null!;
    
    [Inject]
    private ICommandService CommandService { get; set; } = null!;
    
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    
    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;
    
    [Inject]
    private UiErrorService UiErrorService { get; set; } = null!;
    
    [Inject]
    private ResourceService ResourceService { get; set; } = null!;
    
    private async Task<GridData<Role>> LoadData(GridState<Role> arg)
    {
        var roles = Global
            ? await Repository.GetAllGlobal(arg.PageSize, arg.Page * arg.PageSize)
            : await Repository.GetAllAsync(Team?.Id, arg.PageSize, arg.Page * arg.PageSize);
        
        var count = Global
            ? await Repository.CountAllGlobal()
            : await Repository.CountAsync(Team?.Id);

        var rolesList = roles.ToList();
        var resourceIds = rolesList
            .SelectMany(x => x.Policies.Select(y => y.ResourceId))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        _resources = await ResourceService.GetResources(resourceIds, [ResourceType.Team, ResourceType.Project]);
        
        var gridData = new GridData<Role>
        {
            Items = rolesList,
            TotalItems = count
        };

        return gridData;
    }

    private async Task AddPolicy(Role role)
    {
        var parameters = new DialogParameters<ResourcePolicyDialog> {
            { x => x.Global, Global},
            { x => x.ShowAllPolicies, Global},
            { x => x.ResourceSearchFunc, SearchResources}
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        
        var dialog = await DialogService.ShowAsync<ResourcePolicyDialog>("Apply Policy", parameters, options);
        var result = await dialog.Result;

        if (result == null || result.Canceled)
            return;


        var model = (ResourcePolicyFormModel)result.Data!;
        var policy = new ResourcePolicy
        {
            AccessPolicy = model.Policy!,
            Global = Global,
            ResourceId = model.Resource?.Id,
            ResourceType = model.Resource?.GetResourceType()
        };
        
        role.Policies.Add(policy);
        await SaveChangesUpdate(role);
    }

    private async Task<IEnumerable<IResource>> SearchResources(string? searchString, CancellationToken ct)
    {
        var resources = new List<IResource>();
        resources.AddRange(await ProjectRepository.Search(Team?.Id, search: searchString, limit: 10, ct: ct));

        if (Team != null && (string.IsNullOrEmpty(searchString) || Team.Name.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase)))
            resources.Add(Team);

        return resources;
    }

    private async Task OnPolicyEdit(ResourcePolicy? policy, Role role)
    {
        if (policy == null)
            return;
        
        var parameters = new DialogParameters<ResourcePolicyDialog> {
            { x => x.Global, Global},
            { x => x.ShowAllPolicies, Global},
            { x => x.ResourceSearchFunc, SearchResources},
            {
                x => x.Model, new ResourcePolicyFormModel
                {
                    Policy = policy.AccessPolicy,
                    Resource = await ResourceService.GetResource(policy.ResourceType, policy.ResourceId),
                }
            }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };
        
        var dialog = await DialogService.ShowAsync<ResourcePolicyDialog>("Apply Policy", parameters, options);
        var result = await dialog.Result;

        if (result == null || result.Canceled)
            return;
        
        var model = (ResourcePolicyFormModel)result.Data!;
        policy.AccessPolicy = model.Policy!;
        policy.ResourceId = model.Resource?.Id;
        policy.ResourceType = model.Resource?.GetResourceType();
        
        await SaveChangesUpdate(role);
    }

    private async Task OnPolicyDelete(ResourcePolicy? policy, Role role)
    {
        if (policy == null || !await BlazorUtility.ConfirmDelete(DialogService, "policy"))
            return;
        
        role.Policies.Remove(policy);
        await SaveChangesUpdate(role);
    }

    private string GetPolicyResourceName(ResourcePolicy policy)
    {
        if (policy.Global)
            return "Global";

        if (!policy.ResourceId.HasValue)
            return "General";
        
        var resource = _resources.FirstOrDefault(x => x.Id == policy.ResourceId);
        return resource?.Name ?? "Unknown";
    }
    
    private string GetPolicyResourceIcon(ResourcePolicy policy)
    {
        if (policy.Global || !policy.ResourceId.HasValue)
            return Icons.Material.Filled.DataArray;
        
        var resource = _resources.FirstOrDefault(x => x.Id == policy.ResourceId);
        return resource?.GetResourceType().GetIcon() 
               ?? Icons.Material.Filled.QuestionMark;
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
        role.TeamId = Team?.Id;
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