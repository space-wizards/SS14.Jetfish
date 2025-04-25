using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SS14.Jetfish.Projects.Model.FormModel;

namespace SS14.Jetfish.Components.Shared.Forms;

public partial class EditProjectForm : ComponentBase
{
    [Parameter] 
    public EventCallback<EditContext> OnValidSubmit { get; set; }
    
    [Parameter]
    public ProjectFormModel? Model { get; set; }
    
    private ProjectFormModel _model = null!;
    private EditForm _form = null!;

    protected override void OnParametersSet()
    {
        _model = Model ?? new ProjectFormModel();
    }
    
    /// <summary>
    /// Validates the model before returning it
    /// </summary>
    /// <returns>False if the model is invalid</returns>
    public bool TryGetModel([NotNullWhen(true)] out ProjectFormModel? model)
    {
        var valid = _form.EditContext?.Validate() ?? false;
        model = valid ? _model : null;
        return valid;
    }
}