using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MessagePipe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using SS14.Jetfish.Projects.Events;
using SS14.Jetfish.Components.Shared.Dialogs;
using SS14.Jetfish.Core.Events;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Database;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Model.FormModel;
using SS14.Jetfish.Projects.Repositories;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Pages.Projects;

public partial class ProjectPage : ComponentBase, IDisposable
{
    [Inject]
    private ApplicationDbContext Context { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private ProjectRepository Repository { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    [CascadingParameter]
    public Security.Model.User? User { get; set; }

    [Parameter]
    public Guid ProjectId { get; set; }

    [Inject]
    private IConcurrentEventBus EventBus { get; set; } = null!;

    [Inject]
    private ILogger<ProjectPage> Logger { get; set; } = null!;

    private Project? Project { get; set; }

    private MudDropContainer<TaskItem>? _dropContainer;
    private readonly List<Section> _sections = [];
    private List<TaskItem> _tasks = [];

    private bool _addSectionOpen;
    private readonly KanBanNewForm _newSectionModel = new KanBanNewForm();

    private IDisposable? _subscriptions = null;
    private Guid _nextState = Guid.Empty;
    private bool _isLoading = true;

    private void SetupSubscriptions()
    {
        var subscriptions = DisposableBag.CreateBuilder();
        EventBus.Subscribe<CardCreatedEvent>(ProjectId, OnCardCreated).AddTo(subscriptions);
        EventBus.Subscribe<CardMovedEvent>(ProjectId, OnCardMove).AddTo(subscriptions);
        EventBus.Subscribe<LaneCreatedEvent>(ProjectId, OnLaneCreated).AddTo(subscriptions);
        EventBus.Subscribe<LaneRemovedEvent>(ProjectId, OnLaneRemoved).AddTo(subscriptions);
        EventBus.Subscribe<LaneUpdatedEvent>(ProjectId, OnLaneUpdated).AddTo(subscriptions);
        EventBus.Subscribe<CardUpdatedEvent>(ProjectId, OnCardUpdated).AddTo(subscriptions);
        EventBus.Subscribe<ProjectUpdatedEvent>(ProjectId, OnProjectUpdated).AddTo(subscriptions);
        _subscriptions = subscriptions.Build();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _subscriptions?.Dispose();
    }

    private async ValueTask OnProjectUpdated(ProjectUpdatedEvent e, CancellationToken ct)
    {
        if (!await CheckState(e))
            return;


        await InvokeAsync(Refresh);
    }

    private async ValueTask OnCardUpdated(CardUpdatedEvent e, CancellationToken ct)
    {
        // We need to find the card in the tasks list
        var card = _tasks.FirstOrDefault(x => x.Id == e.CardId);

        if (card == null)
            return; // ignore, we apperantly do not have the card. State is probably fucked, will be refreshed by other events

        card.Description = e.Description;
        card.Title = e.Title;

        await InvokeAsync(RefreshContainer);
    }

    private async ValueTask OnLaneUpdated(LaneUpdatedEvent e, CancellationToken ct)
    {
        if (!await CheckState(e))
            return;

        foreach (var task in _tasks.Where(x => x.ListId == e.LaneId))
        {
            task.ListTitle = e.Title;
        }

        var section = _sections.First(x => x.BackingLane.Id == e.LaneId);

        section.BackingLane.Title = e.Title;
        section.EditLaneName = e.Title;

        await InvokeAsync(RefreshContainer);
    }

    private async ValueTask OnLaneRemoved(LaneRemovedEvent e, CancellationToken ct)
    {
        if (!await CheckState(e))
            return;

        _sections.Remove(_sections.First(x => x.BackingLane.Id == e.ListId));

        _tasks.RemoveAll(x => x.ListId == e.ListId);
        await InvokeAsync(RefreshContainer);
    }

    private async ValueTask OnLaneCreated(LaneCreatedEvent e, CancellationToken ct)
    {
        if (!await CheckState(e))
            return;

        _sections.Add(new Section(new SectionLane()
            {
                Title = e.Title,
                Id = e.ListId,
            },
            false,
            string.Empty));

        await InvokeAsync(RefreshContainer);
    }

    private async ValueTask<bool> CheckState(ConcurrentEvent e)
    {
        if (_nextState == Guid.Empty)
        { // This is the first event we recieved, assume its godo
            _nextState = e.NextStateId;
            Logger.LogDebug("Received next state: {state}", _nextState);
            return true;
        }

        if (_nextState == e.StateId)
        {
            // we godo
            _nextState = e.NextStateId;
            Logger.LogDebug("Received next state: {state}", _nextState);
            return true;
        }

        // we godon't
        Logger.LogWarning("State synchronization failed! We are behind! Ours: {our} Theirs: {theirs}", _nextState, e.NextStateId);
        await InvokeAsync(Refresh);
        return false;
    }

    private async ValueTask OnCardCreated(CardCreatedEvent cardCreatedEvent, CancellationToken ct)
    {
        if (!await CheckState(cardCreatedEvent))
            return;

        _tasks.Add(new TaskItem(cardCreatedEvent.Card));
        await InvokeAsync(RefreshContainer);
    }

    private async ValueTask OnCardMove(CardMovedEvent cardMovedEvent, CancellationToken ct)
    {
        if (!await CheckState(cardMovedEvent))
            return;

        // we first find the card that was moved
        var card = _tasks.First(t => t.Id == cardMovedEvent.CardId);
        card.ListId = cardMovedEvent.NewListId;
        card.ListTitle = _sections.First(x => x.BackingLane.Id == cardMovedEvent.NewListId).BackingLane.Title;

        foreach (var task in _tasks)
        {
            if (!cardMovedEvent.Orders.TryGetValue(task.Id, out var order))
                continue;

            task.Order = order;
        }

        _tasks = _tasks.OrderBy(t => t.Order).ToList();

        await InvokeAsync(RefreshContainer);
    }


    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    [Inject]
    private IAuthorizationService AuthorizationService { get; set; } = null!;

    public bool CanPlayTetrisWithCards = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await Refresh();
        SetupSubscriptions();

        if (AuthenticationState != null)
        {
            await AuthenticationState;
            var authorizationResult = await AuthorizationService.AuthorizeAsync(AuthenticationState.Result.User, ProjectId, nameof(Permission.ProjectCardEdit));
            CanPlayTetrisWithCards = authorizationResult.Succeeded;
        }
    }

    private async Task Refresh()
    {
        Logger.LogDebug("Refresh called");
        _sections.Clear();
        _tasks.Clear();

        Project = await Context.Project
            .AsNoTracking() // mudblazor stuff works via ref params, we dont want to accidentally edit the db
            .Include(project =>
                project.Lists)
            .ThenInclude(list => list.Cards)
            .AsSplitQuery()
            .SingleOrDefaultAsync(p => p.Id == ProjectId);

        if (Project != null)
        {
            Validator.ValidateObject(Project, new ValidationContext(Project), true);
            if (Project.BackgroundSpecifier == ProjectBackgroundSpecifier.Color)
                _displaySkeleton = false;

            foreach (var projectList in Project.Lists.OrderBy(list => list.Order))
            {
                _sections.Add(new Section(new SectionLane()
                {
                    Id = projectList.ListId,
                    Title = projectList.Title,
                },false, string.Empty));

                foreach (var card in projectList.Cards.OrderBy(card => card.Order))
                {
                    _tasks.Add(new TaskItem(card));
                }
            }
        }

        _nextState = EventBus.GetState(ProjectId);
        _isLoading = false;
        Logger.LogDebug("Received next state: {state}", _nextState);

        RefreshContainer();
    }

    private void RefreshContainer()
    {
        //update the binding to the container
        StateHasChanged();

        //the container refreshes the internal state
        // We use conditional access here as the drop container is null when the person has no access to the project,
        // as the drop container is never rendered
        _dropContainer?.Refresh();
    }

    private bool _displaySkeleton = true;
    private bool _isImageLoaded = false;

    private void OnImageLoaded()
    {
        _isImageLoaded = true;
        _displaySkeleton = false;
        StateHasChanged(); // trigger re-render
    }

    private string GetBackground()
    {
        if (!_isImageLoaded && Project!.BackgroundSpecifier == ProjectBackgroundSpecifier.Image)
        {
            return "background-color: #000;";
        }

        return Project!.BackgroundSpecifier == ProjectBackgroundSpecifier.Color
            ? $"background: {Project.Background};"
            : $"background: center / cover url(\"project-file/{Project.Id}/file/{Project.Background}\")";
    }

    private async Task CardUpdated(MudItemDropInfo<TaskItem> info)
    {
        var targetZoneId = info.DropzoneIdentifier;
        var droppedCard = info.Item;

        if (droppedCard == null || string.IsNullOrWhiteSpace(targetZoneId))
            return;

        var targetZoneCards = _tasks
            .Where(c => c.ListId.ToString() == targetZoneId)
            .OrderBy(c => c.Order)
            .ToList();

        var newIndex = info.IndexInZone >= 0
            ? info.IndexInZone
            : targetZoneCards.IndexOf(droppedCard);

        await Repository.UpdateCardPosition(ProjectId, info.DropzoneIdentifier, User!.Id, info.Item!.Id, newIndex);
    }

    private async Task AddTask(Section section)
    {
        var card = await Repository.AddCard(ProjectId, section.BackingLane.Id, section.NewTaskName, User!.Id);
        section.NewTaskName = string.Empty;
        section.NewTaskOpen = false;
        //_tasks.Add(new TaskItem(card));
        //RefreshContainer();
    }

    private async Task DeleteSection(Section section)
    {
        var delete = await DialogService.ShowMessageBox(new MessageBoxOptions()
        {
            Title = "Confirm delete",
            Message = "Are you sure you want to delete this lane? This will delete any tasks in the lane.",
            YesText = "Yes",
            NoText = "Cancel",
        });

        if (delete.HasValue && !delete.Value)
            return;

        await Repository.DeleteLane(ProjectId, section.BackingLane.Id);
    }

    public class KanBanNewForm
    {
        [Required]
        [StringLength(Lane.ListTitleMaxLength, ErrorMessage = "Name length can't be more than 120 characters.")]
        public string Name { get; set; } = null!;
    }

    private async Task OnValidSectionSubmit(EditContext context)
    {
        await Repository.AddLane(ProjectId, _newSectionModel.Name);
        _newSectionModel.Name = string.Empty;
        _addSectionOpen = false;
    }

    private async Task OpenCard(TaskItem item)
    {
        await DialogService.ShowAsync<CardDialog>(item.Title, new DialogParameters()
        {
            { nameof(CardDialog.CardId), item.Id },
        }, new DialogOptions()
        {
            BackdropClick = true,
            MaxWidth = MaxWidth.Medium,
            NoHeader = true,
        });
    }

    private async Task SetLaneName(Section item)
    {
        if (item.EditLaneName == item.BackingLane.Title)
        {
            // We exited without any changes.
            item.EditLaneOpen = false;
            return;
        }

        if (_sections.Any(x => x.BackingLane.Title == item.EditLaneName))
        {
            item.EditLaneError = "Lane title must be unique";
            return; // Can't rename to other section.
        }

        item.EditLaneOpen = false;
        item.EditLaneError = null!;
        await Repository.SetLaneName(ProjectId, item.BackingLane.Id, item.EditLaneName);
    }

    private void OpenAddNewSection()
    {
        _addSectionOpen = true;
    }

    public class Section
    {
        public SectionLane BackingLane { get; init; }
        public bool NewTaskOpen { get; set; }
        public string NewTaskName { get; set; }
        public bool EditLaneOpen { get; set; }
        public string EditLaneName { get; set; }
        public string? EditLaneError { get; set; }

        public Section(SectionLane backingLane, bool newTaskOpen, string newTaskName)
        {
            BackingLane = backingLane;
            NewTaskOpen = newTaskOpen;
            NewTaskName = newTaskName;
            EditLaneName = backingLane.Title;
        }
    }

    public class SectionLane
    {
        public required string Title { get; set; }
        public required int Id { get; set; }
    }

    public class TaskItem
    {
        public required float Order { get; set; }
        public required int ListId { get; set; }
        public required string Title { get; set; }
        public required string ListTitle { get; set; }
        public string? Description { get; set; }
        public Guid Id { get; set; }

        [SetsRequiredMembers]
        public TaskItem(Card card)
        {
            Order = card.Order;
            Title = card.Title;
            ListId = card.ListId;
            ListTitle = card.Lane.Title;
            Description = card.Description;
            Id = card.Id;
        }
    }

    private async Task OpenEditDialog()
    {
        var team = Context.Team
            .Include(x => x.Projects)
            .First(x => x.Projects.Any(y => y.Id == ProjectId));

        var model = new ProjectFormModel()
        {
            UserId = User!.Id,
            Team = team,
            Name = Project!.Name,
            BackgroundSpecifier = Project!.BackgroundSpecifier
        };

        if (Project!.BackgroundSpecifier == ProjectBackgroundSpecifier.Color)
            model.BackgroundColor = Project!.Background;

        var parameter = new DialogParameters<EditProjectDialog>()
        {
            { x => x.Model, model },
            { x => x.ProjectId, Project!.Id }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };

        var dialogResult = await DialogService.ShowAsync<EditProjectDialog>("Edit Project", parameter, options);
        var result = await dialogResult.Result;

        if (result == null || result.Canceled || result.Data is not ProjectFormModel modelResult)
            return;

        StateHasChanged();
    }

    private bool CheckCanDragAndDrop(TaskItem arg)
    {
        return CanPlayTetrisWithCards;
    }
}
