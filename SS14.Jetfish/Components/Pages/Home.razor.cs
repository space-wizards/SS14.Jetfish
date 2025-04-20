using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Components.Pages;

[UsedImplicitly]
public partial class Home : ComponentBase
{
    [Inject]
    private TeamRepository UserRepository { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationState { get; set; }

    private ICollection<Team> _teams = [];

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        await base.OnInitializedAsync();
    }

    private async Task LoadData()
    {
        if (AuthenticationState == null)
            throw new InvalidOperationException("AuthenticationState is null");

        var auth = await AuthenticationState;
        var userId = auth.User.Claims.GetUserId();
        if (!userId.HasValue)
            throw new InvalidOperationException("UserId is null");

        _teams = await UserRepository.ListByMembership(userId.Value);
    }
}