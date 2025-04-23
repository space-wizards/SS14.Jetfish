using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Components.Shared.Forms;
using SS14.Jetfish.Security;
using SS14.Jetfish.Security.Model.FormModel;

namespace SS14.Jetfish.Components.Shared.Dialogs;

public partial class ResourcePolicyDialog : ComponentBase
{
    private ResourcePolicyForm _form = null!;

    [CascadingParameter]
    private IMudDialogInstance Dialog { get; set; } = null!;
    
    [Parameter]
    public ResourcePolicyFormModel Model { get; set; } = null!;
    
    [Parameter]
    public bool ShowAllPolicies { get; set; }
    
    [Parameter]
    public bool Global { get; set; }
    
    [Parameter]
    public Func<string?, CancellationToken, Task<IEnumerable<IResource>>?>? ResourceSearchFunc { get; set; } 
    
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
}