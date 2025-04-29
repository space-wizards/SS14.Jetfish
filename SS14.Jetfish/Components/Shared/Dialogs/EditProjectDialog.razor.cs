using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SS14.Jetfish.Components.Shared.Forms;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Projects.Commands;
using SS14.Jetfish.Projects.Model.FormModel;

namespace SS14.Jetfish.Components.Shared.Dialogs;

public partial class EditProjectDialog : ComponentBase
{
    [Inject]
    private ICommandService CommandService { get; set; } = null!;
    
    [Inject]
    private UiErrorService UiErrorService { get; set; } = null!;
    
    [Parameter]
    public ProjectFormModel? Model { get; set; }
    
    [Parameter]
    public Guid ProjectId { get; set; }
    
    private ProjectFormModel _model = null!;
    
    private ProjectForm _form = null!;
    
    [CascadingParameter]
    private IMudDialogInstance Dialog { get; set; } = null!;

    protected override void OnParametersSet()
    {
        _model = Model ?? new ProjectFormModel();
    }

    private void Cancel()
    {
        Dialog.Cancel();
    }

    private async Task Save()
    {
        if (!_form.TryGetModel(out var model))
            return;
        
        var command = new UpdateProjectCommand(ProjectId, model);
        var commandResult = await CommandService.Run(command);
        if (!commandResult!.Result!.IsSuccess)
        {
            await UiErrorService.HandleUiError(commandResult.Result.Error);
            return;
        }
        
        Dialog.Close(DialogResult.Ok(model));
    }
}