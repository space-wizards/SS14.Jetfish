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
    public Guid? EditingState { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        TaskDetails = await ProjectRepository.GetCard(CardId);

        var subscriptions = DisposableBag.CreateBuilder();
        EventBus.Subscribe<CommentAddedEvent>(CardId, OnCommentAdded).AddTo(subscriptions);
        EventBus.Subscribe<CommentEditedEvent>(CardId, OnCommentEdited).AddTo(subscriptions);
        _subscriptions = subscriptions.Build();

        StateHasChanged();
    }

    private async ValueTask OnCommentAdded(CommentAddedEvent e, CancellationToken ct)
    {
        if (e.StateId == EditingState)
            EditingState = e.NextStateId;

        TaskDetails?.Comments.Add(e.Comment);

        await InvokeAsync(StateHasChanged);
    }

    private async ValueTask OnCommentEdited(CommentEditedEvent e, CancellationToken ct)
    {
        var comment = TaskDetails?.Comments.FirstOrDefault(x => x.Id == e.Comment.Id);
        if (comment == null)
            return;

        if (e.StateId == EditingState)
            EditingState = e.NextStateId;

        comment.Content = e.Comment.Content;

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _subscriptions?.Dispose();
    }

    private async Task StartEditingComment(CardComment comment, Guid? state)
    {
        PreviousEditorText = await (_editor?.GetText() ?? Task.FromResult<string?>(null));
        CurrentEditorText = comment.Content;
        CurrentlyEditedComment = comment;
        EditingState = state;
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
                EditingState!.Value,
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
}

