using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Serilog;
using SS14.Jetfish.Components.Shared.Forms;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Commands;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Model.FormModel;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Model.FormModel;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Shared.Dialogs;

public partial class CreateProjectDialog : ComponentBase
{
    [Inject]
    private  ICommandService CommandService { get; set; } = null!;

    [Inject]
    private  ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private  UiErrorService UiErrorService { get; set; } = null!;

    [Inject]
    private IServiceScopeFactory ScopeFactory { get; set; } = null!;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [CascadingParameter]
    private  Task<AuthenticationState>? AuthenticationState { get; set; }

    [Parameter]
    public Team Team { get; set; } = null!;

    private readonly ProjectFormModel _model = new()
    {
        BackgroundColor = ColorGenerator.Hex(),
    };

    private ProjectForm _projectForm = null!;
    private MudDataGrid<Role> _roleGrid = null!;

    private Dictionary<Guid, ResourcePolicyFormModel> _rolePolicies = new();

    private bool _displayProgressbar;

    private void Cancel() => MudDialog.Cancel();

    protected override async Task OnParametersSetAsync()
    {
        _model.Team = Team;
        _model.UserId = await AuthenticationState.GetUserId();
    }

    private async Task Save()
    {
        _displayProgressbar = true;
        StateHasChanged();

        if (!_projectForm.TryGetModel(out var model))
        {
            _displayProgressbar = false;
            return;
        }

        var command = new CreateProjectCommand(model);
        var commandResult = await CommandService.Run(command);


        if (!commandResult!.Result!.IsSuccess)
        {
            _displayProgressbar = false;
            await UiErrorService.HandleUiError(commandResult.Result.Error);
            return;
        }

        if (await ProcessRolePolicies(commandResult.Result.Value))
        {
            _displayProgressbar = false;
            return;
        }

        _displayProgressbar = false;
        Snackbar.Add("Changes Saved!", Severity.Success);
        MudDialog.Close();
    }

    private async Task<bool> ProcessRolePolicies(Project project)
    {
        await using var scope = ScopeFactory.CreateAsyncScope();
        var roleRepository = scope.ServiceProvider.GetRequiredService<RoleRepository>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        foreach (var rolePolicy in _rolePolicies)
        {
            // I fucking hate efcore so much (╯°□°)╯︵ ┻━┻
            context.ChangeTracker.Clear();
            var role = await roleRepository.GetAsync(rolePolicy.Key);
            if (role == null)
                continue;


            if (rolePolicy.Value.Policy == null)
                return true;

            var policy = new ResourcePolicy
            {
                AccessPolicy = rolePolicy.Value.Policy,
                Global = false,
                ResourceId = project.Id,
                ResourceType = ResourceType.Project
            };

            role.Policies.Add(policy);
            var result  = await roleRepository.AddOrUpdate(role);

            if (!result.IsSuccess)
            {
                await UiErrorService.HandleUiError(result.Error);
                return true;
            }
        }

        return false;
    }

    private async Task<GridData<Role>> LoadRoles(GridStateVirtualize<Role> virtualize, CancellationToken ct)
    {
        await using var scope = ScopeFactory.CreateAsyncScope();
        var roleRepository = scope.ServiceProvider.GetRequiredService<RoleRepository>();

        var count = await roleRepository.CountAsync(Team.Id, ct);
        if (count == 0)
            return new GridData<Role>();

        var roles = await roleRepository.GetAllAsync(Team.Id, virtualize.Count, virtualize.StartIndex, ct);

        return new GridData<Role>
        {
            Items = roles,
            TotalItems = count
        };
    }

    private async Task OnResourcePolicyEdit(Role? role)
    {
        if (role == null)
            return;

        _rolePolicies.TryGetValue(role.Id, out var roleModel);

        var parameters = new DialogParameters<ResourcePolicyDialog> {
            { x => x.FixedResource, Team},
            { x => x.Model, roleModel}
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };

        var dialog = await DialogService.ShowAsync<ResourcePolicyDialog>("Apply Policy", parameters, options);
        var result = await dialog.Result;

        if (result == null || result.Canceled || result.Data is not ResourcePolicyFormModel model)
            return;

        _rolePolicies.Add(role.Id, model);

        await Task.CompletedTask;
    }

    private bool HasResourcePolicy(Role role)
    {
        return _rolePolicies.ContainsKey(role.Id);
    }

    private Task OnResourcePolicyDelete(Role role)
    {
        _rolePolicies.Remove(role.Id);
        return Task.CompletedTask;
    }
}
