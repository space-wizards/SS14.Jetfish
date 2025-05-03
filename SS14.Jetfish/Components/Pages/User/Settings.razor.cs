using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace SS14.Jetfish.Components.Pages.User;

[UsedImplicitly]
public partial class Settings : ComponentBase
{
    [CascadingParameter]
    public Security.Model.User? User { get; set; }
}