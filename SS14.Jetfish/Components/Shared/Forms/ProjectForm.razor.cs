using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Model.FormModel;

namespace SS14.Jetfish.Components.Shared.Forms;

public partial class ProjectForm : ComponentBase
{
    [Inject]
    private IOptions<ServerConfiguration> ServerConfiguration { get; set; } = null!;
    
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
        if (!CheckFileSize())
        {
            model = null;
            return false;
        }
        
        var valid = _form.EditContext?.Validate() ?? false;
        model = valid ? _model : null;
        return valid;
    }

    private bool CheckFileSize()
    {
        if (_model.BackgroundSpecifier == ProjectBackgroundSpecifier.Color || _model.BackgroundFile == null || _model.BackgroundFile.Size <= ServerConfiguration.Value.MaxUploadSize) 
            return true;
        
        _fileError = $"Maximum upload size of {ServerConfiguration.Value.MaxUploadSize} exceeded!";
        StateHasChanged();
        return false;
    }

    private void FilesChanges()
    {
        if (CheckFileSize())
            _fileError = string.Empty;
    }
}