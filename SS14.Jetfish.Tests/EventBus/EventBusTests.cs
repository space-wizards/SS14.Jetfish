using SS14.Jetfish.Core.Events;
using SS14.Jetfish.Core.Types;

[assembly: CaptureConsole]
namespace SS14.Jetfish.Tests.EventBus;

public class EventBusTests : IClassFixture<EventBusFixture>
{
    private readonly EventBusFixture _fixture;

    public EventBusTests(EventBusFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ConcurrentEventSuccess()
    {
        var eventBus = _fixture.EventBus;

        var key = Guid.NewGuid();
        Guid? state = null;
        var sub = eventBus.Subscribe<TestEvent>(key, (s, e) =>
        {
            state = s.NextStateId;
            return ValueTask.CompletedTask;
        });

        await eventBus.PublishAsync(key, new TestEvent(), TestContext.Current.CancellationToken);
        sub.Dispose();

        Assert.NotNull(state);

        var result = await eventBus.CallSynced(
            key,
            state.Value,
            () => Task.FromResult(Result<Core.Types.Void, Exception>.Success(Core.Types.Void.Nothing)));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ConcurrentEventFailure()
    {
        var eventBus = _fixture.EventBus;

        var key = Guid.NewGuid();
        await eventBus.PublishAsync(key, new TestEvent(), TestContext.Current.CancellationToken);

        var result = await eventBus.CallSynced(
            key,
            Guid.NewGuid(),
            () => Task.FromResult(Result<Core.Types.Void, Exception>.Success(Core.Types.Void.Nothing)));

        Assert.False(result.IsSuccess);
    }

    private class TestEvent : ConcurrentEvent;
}
