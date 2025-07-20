using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Components.Shared.Markdown;
using SS14.Jetfish.Projects.Hubs;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Shared.Dialogs;

public partial class CardDialog : ComponentBase, IDisposable
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject]
    private ProjectRepository ProjectRepository { get; set; } = null!;

    [CascadingParameter]
    private User? User { get; set; }

    [Inject]
    private ProjectHub Hub { get; set; } = null!;

    /// <summary>
    /// The card to load.
    /// </summary>
    [Parameter]
    public Guid CardId { get; set; }

    private bool IsLoaded { get; set; } = false;
    private Card? Card { get; set; }
    private Dictionary<string, int> Lists { get; set; } = new Dictionary<string, int>();
    private string CardLaneTitle { get; set; } = string.Empty;
    public MarkdownEditor CommentEditor { get; set; } = null!;

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

        Hub.RegisterHandler<CardUpdatedEvent>(CardUpdated);
        Hub.RegisterHandler<CommentAddedEvent>(OnCommentAdded);
        Hub.RegisterHandler<CommentEditedEvent>(OnCommentEdited);

        IsLoaded = true;
        StateHasChanged();
    }


    public void Dispose()
    {
        Hub.UnregisterHandler<CardUpdatedEvent>(CardUpdated);
        Hub.UnregisterHandler<CommentAddedEvent>(OnCommentAdded);
        Hub.UnregisterHandler<CommentEditedEvent>(OnCommentEdited);
    }

    private async Task OnCommentEdited(object sender, CommentEditedEvent e)
    {
        var comment = Card!.Comments.FirstOrDefault(x => x.Id == e.Comment.Id);
        if (comment == null)
            return;

        comment.Content = e.Comment.Content;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnCommentAdded(object sender, CommentAddedEvent e)
    {
        if (((Guid, Guid))sender != (Card!.Id, Card!.ProjectId))
            return;

        Card.Comments.Add(e.Comment);
        await InvokeAsync(StateHasChanged);
    }

    private async Task CardUpdated(object sender, CardUpdatedEvent card)
    {
        if (((Guid, Guid))sender != (Card!.Id, Card!.ProjectId))
            return;

        Card = card.Card;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnEditCardDescription(string description)
    {
        // i *think* this card is not tracked by ef
        Card!.Description = description;
        await ProjectRepository.UpdateCardLite(Card!);
    }

    private async Task CommentSubmit(string text)
    {
        await ProjectRepository.AddComment(Card!.Id, User!, text);
        await CommentEditor.Reset();
    }

    private async Task OnEditComment(CardComment comment, string newText)
    {
        await ProjectRepository.EditComment(comment.Id, newText);
    }
}
