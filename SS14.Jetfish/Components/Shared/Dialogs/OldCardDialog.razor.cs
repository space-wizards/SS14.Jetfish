using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using SS14.Jetfish.Components.Shared.Markdown;
using SS14.Jetfish.Core.Events;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Events;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Shared.Dialogs;

public partial class OldCardDialog : ComponentBase, IDisposable
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

    private bool IsLoaded { get; set; } = false;
    private Card? Card { get; set; }
    private Dictionary<string, int> Lists { get; set; } = new Dictionary<string, int>();
    private string CardLaneTitle { get; set; } = string.Empty;
    public MarkdownEditor CommentEditor { get; set; } = null!;
    private bool _isEditing;

    private IDisposable? _subscriptions;
    private Guid _cardState = Guid.Empty;

    private bool _editTitleOpen = false;
    private string? _editTitleError = null;

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

        var subscriptions = DisposableBag.CreateBuilder();
        EventBus.Subscribe<CardUpdatedEvent>(CardId, CardUpdated).AddTo(subscriptions);
        EventBus.Subscribe<CommentAddedEvent>(CardId, OnCommentAdded).AddTo(subscriptions);
        EventBus.Subscribe<CommentEditedEvent>(CardId, OnCommentEdited).AddTo(subscriptions);
        EventBus.Subscribe<CommentDeletedEvent>(CardId, OnCommentDeleted).AddTo(subscriptions);
        EventBus.Subscribe<CardMovedEvent>(CardId, OnCardMoved).AddTo(subscriptions);

        _cardState = EventBus.GetState(CardId);
        _subscriptions = subscriptions.Build();

        IsLoaded = true;
        StateHasChanged();
    }

    /// <summary>
    /// Attempts to set <see cref="_cardState"/> to the current value. Will do nothing if we are behind.
    /// </summary>
    private void AttemptStateSynchronize(ConcurrentEvent e)
    {
        if (e.StateId == _cardState)
            _cardState = e.NextStateId;
    }

    public void Dispose()
    {
        _subscriptions?.Dispose();
    }

    private async ValueTask OnCardMoved(CardMovedEvent e, CancellationToken ct)
    {
        if (Card == null)
            return;
        AttemptStateSynchronize(e);

        var newList = Lists.FirstOrDefault(x => x.Value == e.NewListId);
        Card.ListId = newList.Value;
        Card.Lane.Title = newList.Key;
        CardLaneTitle = newList.Key;
        await InvokeAsync(StateHasChanged);
    }

    private async ValueTask OnCommentEdited(CommentEditedEvent e, CancellationToken ct)
    {
        var comment = Card?.Comments.FirstOrDefault(x => x.Id == e.Comment.Id);
        if (comment == null)
            return;
        AttemptStateSynchronize(e);

        comment.Content = e.Comment.Content;
        await InvokeAsync(StateHasChanged);
    }

    private async ValueTask OnCommentDeleted(CommentDeletedEvent e, CancellationToken ct)
    {
        AttemptStateSynchronize(e);

        Card?.Comments.Remove(Card!.Comments.First(x => x.Id == e.CommentId));
        await InvokeAsync(StateHasChanged);
    }

    private async ValueTask OnCommentAdded(CommentAddedEvent e, CancellationToken ct)
    {
        AttemptStateSynchronize(e);

        Card?.Comments.Add(e.Comment);
        await InvokeAsync(StateHasChanged);
    }

    private async ValueTask CardUpdated(CardUpdatedEvent e, CancellationToken ct)
    {
        AttemptStateSynchronize(e);

        Card = await ProjectRepository.GetCard(CardId);
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnEditCardDescription(string description)
    {
        if (Card == null)
            return;

        // I *think* this card is not tracked by ef
        Card.Description = description;
        var result = await EventBus.CallSynced(CardId, _cardState, () => ProjectRepository.UpdateCardLite(Card!));
        if (!result.IsSuccess)
            await UiErrorService.HandleUiError(result.Error);

        _isEditing = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task CommentSubmit(string text)
    {
        var result = await EventBus.CallSynced(CardId, _cardState, () => ProjectRepository.AddComment(Card!.Id, User!, text));
        if (!result.IsSuccess)
            await UiErrorService.HandleUiError(result.Error);

        await CommentEditor.Reset();
    }

    private async Task OnEditComment(CardComment comment, string newText)
    {
        var result = await EventBus.CallSynced(CardId, _cardState, () => ProjectRepository.EditComment(comment.Id, newText));

        if (!result.IsSuccess)
            await UiErrorService.HandleUiError(result.Error);
    }

    private async Task OnDeleteComment(CardComment comment)
    {
        var result = await EventBus.CallSynced(CardId, _cardState, () => ProjectRepository.DeleteComment(comment.Id));

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

    private async Task SetTitle()
    {
        if (Card!.Title.Length > Card.CardTitleMaxLength)
        {
            _editTitleError = $"Title cannot be longer than {Card.CardTitleMaxLength} characters";
            return;
        }

        if (string.IsNullOrWhiteSpace(Card!.Title))
        {
            _editTitleError = "Title cannot be empty";
            return;
        }

        var result = await EventBus.CallSynced(CardId, _cardState, () => ProjectRepository.UpdateCardLite(Card!));
        if (!result.IsSuccess)
            await UiErrorService.HandleUiError(result.Error);

        _editTitleOpen = false;
        _editTitleError = null;
        await InvokeAsync(StateHasChanged);
    }
}
