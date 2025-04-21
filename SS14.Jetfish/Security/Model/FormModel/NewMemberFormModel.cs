using System.ComponentModel.DataAnnotations;

namespace SS14.Jetfish.Security.Model.FormModel;

public class NewMemberFormModel
{
    [Required]
    public Guid TeamId { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid? RoleId { get; set; }
}