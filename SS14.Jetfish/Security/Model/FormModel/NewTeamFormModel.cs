using System.ComponentModel.DataAnnotations;

namespace SS14.Jetfish.Security.Model.FormModel;

public class NewTeamFormModel
{
    
    public const int NameMinLength = 3;
    public const int NameMaxLength = 80;

    [Required]
    [MinLength(NameMinLength, ErrorMessage = "Name must be at least 3 characters")]
    [MaxLength(NameMaxLength, ErrorMessage = "Name cannot exceed 50 characters")]
    public string Name { get; set; } = null!;
    
    [MaxLength(Role.MaxDisplayNameLength, ErrorMessage = "Name cannot exceed 30 characters")]
    public string? ManagerRoleName { get; set; }
    
    [MaxLength(Role.MaxDisplayNameLength, ErrorMessage = "Name cannot exceed 30 characters")]
    public string? MemberRoleName { get; set; }
    
    public bool AddSelf { get; set; }
    public Guid UserId { get; set; }
}