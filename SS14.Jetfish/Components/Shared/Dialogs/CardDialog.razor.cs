using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;

namespace SS14.Jetfish.Components.Shared.Dialogs;

public partial class CardDialog : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject]
    private ProjectRepository ProjectRepository { get; set; } = null!;

    /// <summary>
    /// The card to load.
    /// </summary>
    [Parameter]
    public Guid CardId { get; set; }

    private bool IsLoaded { get; set; } = false;
    private Card? Card { get; set; }
    private Dictionary<string, int> Lists { get; set; } = new Dictionary<string, int>();
    private string CardLaneTitle { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        Card = await ProjectRepository.GetCard(CardId);
        if (Card != null)
        {
            Lists = (await ProjectRepository.GetLanes(Card.ProjectId))
                .ToDictionary(x => x.Title, x => x.ListId);
            CardLaneTitle = Card.Lane.Title;
        }

        IsLoaded = true;
        StateHasChanged();
    }
}
