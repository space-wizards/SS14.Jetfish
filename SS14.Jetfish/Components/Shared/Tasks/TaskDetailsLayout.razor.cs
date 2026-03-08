using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using SS14.Jetfish.Components.Shared.Markdown;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Commands;
using SS14.Jetfish.Projects.Events;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Shared.Tasks;

public partial class TaskDetailsLayout : ComponentBase, IDisposable
{
    [Inject]
    private ProjectRepository ProjectRepository { get; set; } = null!;

    [Inject]
    private UiErrorService UiErrorService { get; set; } = null!;

    [Inject]
    private IConcurrentEventBus EventBus { get; set; } = null!;

    [Inject]
    private ICommandService CommandService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [CascadingParameter]
    private User? User { get; set; }

    private MarkdownEditor? _editor;
    private IDisposable? _subscriptions;

    /// <summary>
    /// The card to load.
    /// </summary>
    [Parameter]
    public Guid CardId { get; set; }

    private Card? TaskDetails { get; set; }

    private string? PreviousEditorText { get; set; }
    private string? CurrentEditorText { get; set; }
    private CardComment? CurrentlyEditedComment { get; set; }
    private Dictionary<Guid, Guid?> StateMap { get; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        TaskDetails = await ProjectRepository.GetCard(CardId);

        var subscriptions = DisposableBag.CreateBuilder();
        EventBus.Subscribe<CommentAddedEvent>(CardId, OnCommentAdded).AddTo(subscriptions);
        EventBus.Subscribe<CommentEditedEvent>(CardId, OnCommentEdited).AddTo(subscriptions);
        EventBus.Subscribe<CommentDeletedEvent>(CardId, OnCommentDeleted).AddTo(subscriptions);
        _subscriptions = subscriptions.Build();

        SyncStates();
        StateHasChanged();
    }

    private void SyncStates()
    {
        if (TaskDetails == null)
            return;

        foreach (var comment in TaskDetails.Comments)
        {
            StateMap[comment.Id] = EventBus.GetState(comment.Id);
        }
    }

    private async ValueTask OnCommentAdded(CommentAddedEvent e, CancellationToken ct)
    {
        StateMap[e.Comment.Id] = e.NextStateId;
        TaskDetails?.Comments.Add(e.Comment);
        await InvokeAsync(StateHasChanged);
    }

    private async ValueTask OnCommentEdited(CommentEditedEvent e, CancellationToken ct)
    {
        var comment = TaskDetails?.Comments.FirstOrDefault(x => x.Id == e.Comment.Id);
        if (comment == null)
            return;

        if (StateMap.TryGetValue(e.Comment.Id, out var state) && state == e.StateId)
            StateMap[e.Comment.Id] = e.NextStateId;

        comment.Content = e.Comment.Content;

        await InvokeAsync(StateHasChanged);
    }

    private async ValueTask OnCommentDeleted(CommentDeletedEvent e, CancellationToken ct)
    {
        var comment = TaskDetails?.Comments.FirstOrDefault(x => x.Id == e.CommentId);
        if (comment == null)
            return;

        TaskDetails?.Comments.Remove(comment);
        StateMap.Remove(e.CommentId);
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _subscriptions?.Dispose();
    }

    private async Task StartEditingComment(CardComment comment)
    {
        PreviousEditorText = await (_editor?.GetText() ?? Task.FromResult<string?>(null));
        CurrentEditorText = comment.Content;
        CurrentlyEditedComment = comment;
    }

    private async Task SaveComment(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var command = new CreateOrUpdateCommentCommand(CardId, text, User!, CurrentlyEditedComment?.Id);
        Result<CardComment, Exception> result;

        if (CurrentlyEditedComment != null)
        {
            result = await EventBus.CallSynced(
                CurrentlyEditedComment.Id,
                StateMap.GetValueOrDefault(CurrentlyEditedComment.Id, Guid.Empty)!.Value,
                async () => (await CommandService.Run(command))!.Result!
                );
        }
        else
        {
            result = (await CommandService.Run(command))!.Result!;
        }

        if (!result.IsSuccess)
        {
            await UiErrorService.HandleUiError(result.Error);
            return;
        }

        CurrentEditorText = PreviousEditorText;
        PreviousEditorText = null;
        CurrentlyEditedComment = null;
        if (CurrentlyEditedComment == null && _editor != null)
            await _editor.ClearText();
    }

    private Task CancelEditingComment()
    {
        CurrentEditorText = PreviousEditorText;
        PreviousEditorText = null;
        CurrentlyEditedComment = null;
        return Task.CompletedTask;
    }

    private async Task DeleteComment(Guid commentId)
    {
        if (CurrentlyEditedComment != null && CurrentlyEditedComment.Id == commentId)
            await CancelEditingComment();

        await EventBus.CallSynced(
            commentId,
            StateMap.GetValueOrDefault(commentId, Guid.Empty)!.Value,
            async () => (await CommandService.Run(new DeleteCommentCommand(commentId)))!.Result!
        );

        Snackbar.Add("Comment deleted", Severity.Success, key: commentId.ToString());
    }
}

