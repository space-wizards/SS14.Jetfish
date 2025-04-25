using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SS14.Jetfish.Components.Shared.Forms;
using SS14.Jetfish.Projects.Model.FormModel;

namespace SS14.Jetfish.Components.Shared.Dialogs;

public partial class EditProjectDialog : ComponentBase
{
    [Parameter]
    public ProjectFormModel? Model { get; set; }
    
    private ProjectFormModel _model = null!;
    
    private EditProjectForm _form = null!;
    
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

    private Task Save()
    {
        if (!_form.TryGetModel(out var model))
            return Task.CompletedTask;
        
        Dialog.Close(DialogResult.Ok(model));
        return Task.CompletedTask;
    }

    private async Task OnValidSubmit()
    {
        if (!_form.TryGetModel(out var model))
            return;

        if (model.BackgroundFile == null)
            return;
    }
}