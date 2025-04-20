using Microsoft.AspNetCore.Components;

namespace SS14.Jetfish.Components.Shared.ProjectCards;

public partial class ProjectCardTemplate : ComponentBase
{
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }
}