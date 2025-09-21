using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Shared.Tasks;

public partial class TaskDetailsLayout : ComponentBase, IDisposable
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject]
    private ProjectRepository ProjectRepository { get; set; } = null!;

    [CascadingParameter]
    private User? User { get; set; }

    [Inject]
    private UiErrorService UiErrorService { get; set; } = null!;

    [Inject]
    private IConcurrentEventBus EventBus { get; set; } = null!;


    /// <summary>
    /// The card to load.
    /// </summary>
    [Parameter]
    public Guid CardId { get; set; }

    private Card? Task { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        Task = await ProjectRepository.GetCard(CardId);
        StateHasChanged();
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}

