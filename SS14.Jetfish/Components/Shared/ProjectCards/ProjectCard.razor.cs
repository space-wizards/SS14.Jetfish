using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Components.Shared.ProjectCards;

public partial class ProjectCard : ComponentBase
{
    [Parameter]
    public required Project Project { get; set; }

    private string GetBackground()
    {
        Validator.ValidateObject(Project, new ValidationContext(Project), true);
        
        if (Project.BackgroundSpecifier == ProjectBackgroundSpecifier.Color)
        {
            return $"background: {Project.Background};";
        }
        else
        {
            return $"background: center / cover url(\"project-file/{Project.Id}/file/{Project.Background}\")";
        }
    }
}