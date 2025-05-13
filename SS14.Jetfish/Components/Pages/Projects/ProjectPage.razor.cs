using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Components.Layout;
using SS14.Jetfish.Database;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Pages.Projects;

public partial class ProjectPage : ComponentBase
{
    [Inject]
    private ApplicationDbContext Context { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    [Parameter]
    public Guid ProjectId { get; set; }

    private Project? Project { get; set;}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        Project = await Context.Project.SingleOrDefaultAsync(p => p.Id == ProjectId);
        if (Project != null)
        {
            Validator.ValidateObject(Project, new ValidationContext(Project), true);
            if (Project.BackgroundSpecifier == ProjectBackgroundSpecifier.Color)
                _displaySkeleton = false;
        }

        StateHasChanged();
    }

    private bool _displaySkeleton = true;
    private bool _isImageLoaded = false;

    private void OnImageLoaded()
    {
        _isImageLoaded = true;
        _displaySkeleton = false;
        StateHasChanged(); // trigger re-render
    }

    private string GetBackground()
    {
        if (!_isImageLoaded && Project!.BackgroundSpecifier == ProjectBackgroundSpecifier.Image)
        {
            return "background-color: #000;";
        }

        return Project!.BackgroundSpecifier == ProjectBackgroundSpecifier.Color
            ? $"background: {Project.Background};"
            : $"background: center / cover url(\"project-file/{Project.Id}/file/{Project.Background}\")";
    }
}
