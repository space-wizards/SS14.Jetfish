using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Model.FormModel;

public class ProjectFormModel
{
    public const int NameMinLength = 3;

    [Required]
    [MinLength(NameMinLength, ErrorMessage = "Name must be at least 3 characters")]
    [MaxLength(Project.ProjectNameMaxLength, ErrorMessage = "Name cannot exceed 64 characters")]
    public string Name { get; set; } = null!;

    public IBrowserFile? BackgroundFile { get; set; }
    public string? BackgroundColor { get; set; }

    [Required]
    public ProjectBackgroundSpecifier BackgroundSpecifier { get; set; } = ProjectBackgroundSpecifier.Color;

    [Required]
    public Team Team { get; set; } = null!;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public bool Public { get; set; }
}
