using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SS14.Jetfish.Projects.Model;
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

    public int BackgroundType
    {
        get
        {
            return _model.BackgroundSpecifier switch
            {
                ProjectBackgroundSpecifier.Color => 0,
                ProjectBackgroundSpecifier.Image => 1,
                _ => 0
            };
        }
        set
        {
            _model.BackgroundSpecifier = value switch
            {
                0 => ProjectBackgroundSpecifier.Color,
                1 => ProjectBackgroundSpecifier.Image,
                _ => ProjectBackgroundSpecifier.Color
            };
        }
    }

    private string _fileError = "";

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
    
    private void FilesChanges()
    {
        _fileError = string.Empty;
    }
}