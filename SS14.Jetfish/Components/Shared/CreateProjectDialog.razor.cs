using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.FileHosting.Services;
using SS14.Jetfish.Helpers;
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

        Team.Projects.Add(new Project()
        {
            Background = ProjectBackgroundColor,
            Name = ProjectName,
            BackgroundSpecifier = SelectedBackgroundSpecifier,
        });

        var command = new CreateOrUpdateTeamCommand(Team);
        var commandResult = await CommandService.Run(command);
        if (!commandResult!.Result!.IsSuccess)
        {
            await BlazorUtility.DisplayModifiedPopup(DialogService, NavigationManager);
        }

        Snackbar.Add("Changes Saved!", Severity.Success);
        MudDialog.Close();
    }
}