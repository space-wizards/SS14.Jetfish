using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Components.Shared.ProjectCards;

public partial class ProjectCard : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter]
    public required Project Project { get; set; }

    protected override void OnParametersSet()
    {
        Validator.ValidateObject(Project, new ValidationContext(Project), true);
        if (Project.BackgroundSpecifier != ProjectBackgroundSpecifier.Color)
            return;

        StateHasChanged();
    }

    private string GetBackground()
    {
        return Project.BackgroundSpecifier == ProjectBackgroundSpecifier.Color
            ? $"background: {Project.Background};"
            : $"background: center / cover url(\"project-file/{Project.Id}/file/{Project.Background}\")";
    }

    private void RedirectToProject()
    {
        NavigationManager.NavigateTo($"projects/{Project.Id}");
    }
}
