using System.ComponentModel.DataAnnotations;
using SS14.Jetfish.Security.Services.Interfaces;

namespace SS14.Jetfish.Security.Model.FormModel;

public sealed class ResourcePolicyFormModel
{
    [Required]
    public AccessPolicy? Policy { get; set; }
    
    public IResource? Resource { get; set; }
}