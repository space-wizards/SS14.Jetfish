using System.ComponentModel.DataAnnotations;

namespace SS14.Jetfish.Security.Model.FormModel;

public sealed class ResourcePolicyFormModel
{
    [Required]
    public AccessPolicy? Policy { get; set; }
    
    public IResource? Resource { get; set; }
}