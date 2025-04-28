using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Serilog;
using SS14.Jetfish.Configuration;
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
    private  IDialogService DialogService { get; set; } = null!;
    [Inject]
    private  NavigationManager NavigationManager { get; set; } = null!;
    [Inject]
    private IConfiguration Configuration { get; set; } = null!;

    private ServerConfiguration ServerConfiguration { get; set; } = new();



    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    private EditForm _form = null!;

    [CascadingParameter]
    private  Task<AuthenticationState>? AuthenticationState { get; set; }

    [Parameter]
    public Team Team { get; set; } = null!;

    private bool _displayProgressbar = false;

    private void Cancel() => MudDialog.Cancel();
    
    protected override async Task OnParametersSetAsync()
    {
        Configuration.Bind(ServerConfiguration.Name, ServerConfiguration);

        _model.Team = Team;
        var auth = await AuthenticationState!; // If this comp is used, I expect some other auth check to have already passed
        var userId = auth.User.Claims.GetUserId()!; // same here fuck you
        _model.UserId = userId.Value;
    }

    private readonly ProjectFormModel _model = new()
    {
        BackgroundColor = $"#{new Random().Next(0x1000000):X6}"
    };

    private string _fileError = "";

    private void FilesChanges()
    {
        _fileError = string.Empty;
    }

    private async Task Save()
    {
        var valid = _form.EditContext?.Validate() ?? false;
        var model = valid ? _model : null;

        if (model == null)
            return;

        if (model.BackgroundFile != null && model.BackgroundFile.Size > ServerConfiguration.MaxUploadSize)
        {
            _fileError = $"Maximum upload size of {ServerConfiguration.MaxUploadSize} exceeded!";
            return;
        }

        _displayProgressbar = true;
        StateHasChanged();

        var command = new CreateProjectCommand(model);
        var commandResult = await CommandService.Run(command);
        if (!commandResult!.Result!.IsSuccess)
        {
            _displayProgressbar = false;

            if (commandResult.Result.Error is IOException)
            {
                Log.Error("File upload failed: {error}", commandResult.Result.Error);
                _fileError = "Failed to upload file!";
                return;
            }

            if (commandResult.Result.Error is DbUpdateConcurrencyException)
            {
                await BlazorUtility.DisplayErrorPopup(DialogService, NavigationManager);
            }

            throw commandResult.Result.Error;
            //Snackbar.Add("Failed to create project", Severity.Error);
            //Log.Error("Failed to create project: {error}", commandResult.Result.Error);
        }

        Snackbar.Add("Changes Saved!", Severity.Success);
        MudDialog.Close();
    }
}