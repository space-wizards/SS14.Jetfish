using Microsoft.AspNetCore.Components;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Components.Shared.ProjectCards;

public partial class ProjectCard : ComponentBase
{
    [Parameter]
    public required Project Project { get; set; }
}