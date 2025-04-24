using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Model.FormModel;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Shared.Forms;

public partial class ResourcePolicyForm : ComponentBase
{
    [Inject]
    private PolicyRepository PolicyRepository { get; set; } = null!;
    
    [Parameter]
    public ResourcePolicyFormModel? Model { get; set; }
    
    [Parameter]
    public bool ShowAllPolicies { get; set; }
    
    [Parameter]
    public bool Global { get; set; }
    
    [Parameter]
    public Func<string?, CancellationToken, Task<IEnumerable<IResource>>?>? ResourceSearchFunc { get; set; } 
    
    private ResourcePolicyFormModel _model = null!;
    private EditForm _form = null!;

    private MudBlazor.Converter<AccessPolicy, string> PolicyConverter => new()
    {
        SetFunc = policy => policy?.Name ?? ""
    };
    
    private MudBlazor.Converter<IResource, string> ResourceConverter => new()
    {
        SetFunc = policy => policy?.Name ?? ""
    };
    
    protected override void OnParametersSet()
    {
        _model = Model ?? new ResourcePolicyFormModel();
    }

    private async Task<IEnumerable<AccessPolicy>>? PolicySearch(string? searchString, CancellationToken ct)
    {
        return await PolicyRepository.GetAllAsync(searchString, ShowAllPolicies, ct: ct);
    }

    private async Task<IEnumerable<IResource>> ResourceSearch(string? searchString, CancellationToken ct)
    {
        if (ResourceSearchFunc == null)
            return [];

        return await ResourceSearchFunc.Invoke(searchString, ct)!;
    }
    
    
    /// <summary>
    /// Validates the model before returning it
    /// </summary>
    /// <returns>False if the model is invalid</returns>
    public bool TryGetModel([NotNullWhen(true)] out ResourcePolicyFormModel? model)
    {
        var valid = _form.EditContext?.Validate() ?? false;
        model = valid ? _model : null;
        return valid;
    }
}