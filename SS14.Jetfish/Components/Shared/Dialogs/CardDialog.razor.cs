using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Components.Shared.Markdown;
using SS14.Jetfish.Core.Services;
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
    private UiErrorService UiErrorService { get; set; } = null!;

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
    private bool _isEditing;

    private Guid _cardState = Guid.Empty;

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
        Hub.RegisterHandler<CommentDeletedEvent>(OnCommentDeleted);
        Hub.RegisterHandler<CardMovedEvent>(OnCardMoved);

        _cardState = Hub.GetNextState((Card!.Id, Card!.ProjectId));

        IsLoaded = true;
        StateHasChanged();
    }

    /// <summary>
    /// Attempts to set <see cref="_cardState"/> to the current value. Will do nothing if we are behind.
    /// </summary>
    private void AttemptStateSynchronize(ProjectEvent e)
    {
        if (e.StateId == _cardState)
            _cardState = e.NextStateId;
    }

    public void Dispose()
    {
        Hub.UnregisterHandler<CardUpdatedEvent>(CardUpdated);
        Hub.UnregisterHandler<CommentAddedEvent>(OnCommentAdded);
        Hub.UnregisterHandler<CommentEditedEvent>(OnCommentEdited);
        Hub.UnregisterHandler<CommentDeletedEvent>(OnCommentDeleted);
        Hub.UnregisterHandler<CardMovedEvent>(OnCardMoved);
    }

    private async Task OnCardMoved(object sender, CardMovedEvent e)
    {
        if (e.CardId != CardId)
            return;

        var newList = Lists.FirstOrDefault(x => x.Value == e.NewListId);
        Card!.ListId = newList.Value;
        Card.Lane.Title = newList.Key;
        CardLaneTitle = newList.Key;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnCommentEdited(object sender, CommentEditedEvent e)
    {
        var comment = Card!.Comments.FirstOrDefault(x => x.Id == e.Comment.Id);
        if (comment == null)
            return;
        AttemptStateSynchronize(e);

        comment.Content = e.Comment.Content;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnCommentDeleted(object sender, CommentDeletedEvent e)
    {
        if (((Guid, Guid))sender != (Card!.Id, Card!.ProjectId))
            return;
        AttemptStateSynchronize(e);

        Card!.Comments.Remove(Card!.Comments.First(x => x.Id == e.CommentId));
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnCommentAdded(object sender, CommentAddedEvent e)
    {
        if (((Guid, Guid))sender != (Card!.Id, Card!.ProjectId))
            return;
        AttemptStateSynchronize(e);

        Card.Comments.Add(e.Comment);
        await InvokeAsync(StateHasChanged);
    }

    private async Task CardUpdated(object sender, CardUpdatedEvent e)
    {
        if (((Guid, Guid))sender != (Card!.Id, Card!.ProjectId))
            return;
        AttemptStateSynchronize(e);

        Card = e.Card;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnEditCardDescription(string description)
    {
        // i *think* this card is not tracked by ef
        Card!.Description = description;
        var result = await Hub.AttemptCallSynced((Card!.Id, Card!.ProjectId), _cardState, () => ProjectRepository.UpdateCardLite(Card!));
        if (!result.IsSuccess)
            await UiErrorService.HandleUiError(result.Error);

        _isEditing = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task CommentSubmit(string text)
    {
        var result = await Hub.AttemptCallSynced((Card!.Id, Card!.ProjectId), _cardState, () => ProjectRepository.AddComment(Card!.Id, User!, text));
        if (!result.IsSuccess)
            await UiErrorService.HandleUiError(result.Error);

        await CommentEditor.Reset();
    }

    private async Task OnEditComment(CardComment comment, string newText)
    {
        var result = await Hub.AttemptCallSynced((Card!.Id, Card!.ProjectId),
            _cardState,
            () => ProjectRepository.EditComment(comment.Id, newText));

        if (!result.IsSuccess)
            await UiErrorService.HandleUiError(result.Error);
    }

    private async Task OnDeleteComment(CardComment comment)
    {
        var result = await Hub.AttemptCallSynced((Card!.Id, Card!.ProjectId),
            _cardState,
            () => ProjectRepository.DeleteComment(comment.Id));

        if (!result.IsSuccess)
            await UiErrorService.HandleUiError(result.Error);
    }

    private void ToggleEdit()
    {
        _isEditing = !_isEditing;
        StateHasChanged();
    }

    private async Task LaneChanged(string newLane)
    {
        await ProjectRepository.UpdateCardPosition(Card!.ProjectId, newLane, User!.Id, CardId, 0);
    }
}
