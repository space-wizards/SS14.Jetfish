using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Serilog;
using SS14.Jetfish.Components.Shared.Forms;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Commands;
using SS14.Jetfish.Projects.Model.FormModel;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Shared.Dialogs;

public partial class CreateProjectDialog : ComponentBase
{
    [Inject]
    private  ICommandService CommandService { get; set; } = null!;
    
    [Inject]
    private  ISnackbar Snackbar { get; set; } = null!;
    
    [Inject]
    private  UiErrorService UiErrorService { get; set; } = null!;
    
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [CascadingParameter]
    private  Task<AuthenticationState>? AuthenticationState { get; set; }

    [Parameter]
    public Team Team { get; set; } = null!;

    private bool _displayProgressbar;

    private void Cancel() => MudDialog.Cancel();
    
    protected override async Task OnParametersSetAsync()
    {
        _model.Team = Team;
        _model.UserId = await AuthenticationState.GetUserId();
    }

    private readonly ProjectFormModel _model = new()
    {
        BackgroundColor = $"#{new Random().Next(0x1000000):X6}"
    };

    private ProjectForm _form = null!;
        
    private async Task Save()
    {
        _displayProgressbar = true;
        StateHasChanged();

        if (!_form.TryGetModel(out var model))
            return;
        
        var command = new CreateProjectCommand(model);
        var commandResult = await CommandService.Run(command);
        _displayProgressbar = false;
        
        if (!commandResult!.Result!.IsSuccess)
        {
            await UiErrorService.HandleUiError(commandResult.Result.Error);
            return;
        }

        Snackbar.Add("Changes Saved!", Severity.Success);
        MudDialog.Close();
    }
}