using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Serilog;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.FileHosting.Model;
using SS14.Jetfish.FileHosting.Services;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Commands;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Shared;

public partial class CreateProjectDialog : ComponentBase
{
    [Inject]
    public ICommandService CommandService { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;
    [Inject]
    public FileService FileService { get; set; } = null!;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    private MudForm _form = null!;

    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationState { get; set; }

    [Parameter]
    public Team Team { get; set; } = null!;

    private bool _success;
    private string[] _errors = [];

    public ProjectBackgroundSpecifier SelectedBackgroundSpecifier { get; set; } = ProjectBackgroundSpecifier.Color;
    public string ProjectName { get; set; } = "";

    public string ProjectBackgroundColor { get; set; } = $"#{new Random().Next(0x1000000):X6}";
    public IBrowserFile? ProjectBackgroundFile { get; set; }

    private bool _displayProgressbar = false;

    private void Cancel() => MudDialog.Cancel();

    private async Task Save()
    {
        await _form.Validate();

        if (_errors.Length > 0)
            return;

        var project = new Project()
        {
            Background = ProjectBackgroundColor,
            Name = ProjectName,
            BackgroundSpecifier = ProjectBackgroundSpecifier.Color,
        };

        var createOrUpdateProjectCommand = new CreateOrUpdateProjectCommand(project);
        var createOrUpdateProjectResult = await CommandService.Run(createOrUpdateProjectCommand);
        if (!createOrUpdateProjectResult!.Result!.IsSuccess)
        {
            await BlazorUtility.DisplayModifiedPopup(DialogService, NavigationManager);
            return;
        }

        project = createOrUpdateProjectResult.Result.Value;

        try
        {
            _displayProgressbar = true; // TODO: Make progress bar work
            StateHasChanged();

            if (SelectedBackgroundSpecifier == ProjectBackgroundSpecifier.Image)
            {
                var file = await UploadImageForProject(project);
                project.BackgroundSpecifier = ProjectBackgroundSpecifier.Image;
                // Null suppression because only way this is null is when it throws or no file is selected.
                project.Background = file!.Id.ToString();

                createOrUpdateProjectCommand = new CreateOrUpdateProjectCommand(project);
                createOrUpdateProjectResult = await CommandService.Run(createOrUpdateProjectCommand);
                if (!createOrUpdateProjectResult!.Result!.IsSuccess)
                {
                    await BlazorUtility.DisplayModifiedPopup(DialogService, NavigationManager);
                    return;
                }

                project = createOrUpdateProjectResult.Result.Value;
            }
        }
        catch (Exception e)
        {
            Log.Error("Failed uploading file: {error}", e);
            Snackbar.Add("File upload failed, solid color used as fallback!", Severity.Error);
        }
        finally
        {
            Team.Projects.Add(project);
        }

        var createOrUpdateTeamCommand = new CreateOrUpdateTeamCommand(Team);
        var createOrUpdateTeamResult = await CommandService.Run(createOrUpdateTeamCommand);
        if (!createOrUpdateTeamResult!.Result!.IsSuccess)
        {
            await BlazorUtility.DisplayModifiedPopup(DialogService, NavigationManager);
        }

        Snackbar.Add("Changes Saved!", Severity.Success);
        MudDialog.Close();
    }

    private async Task<UploadedFile?> UploadImageForProject(Project project)
    {
        if (ProjectBackgroundFile == null)
            return null;

        var auth = await AuthenticationState!; // If this comp is used, I expect some other auth check to have already passed
        var userId = auth.User.Claims.GetUserId()!; // same here fuck you

        var result = await FileService.UploadFileForProject(ProjectBackgroundFile, userId.Value,  project.Id);
        if (!result.IsSuccess)
        {
            throw result.Error!;
        }

        return result.Value;
    }
}