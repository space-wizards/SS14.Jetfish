using Microsoft.Extensions.Options;

namespace SS14.Jetfish.Tests.Types;

public class TestOptionsMonitor<T> : IOptionsMonitor<T>
    where T : class, new()
{
    public TestOptionsMonitor(T currentValue)
    {
        CurrentValue = currentValue;
    }

    public T Get(string name)
    {
        return CurrentValue;
    }

    public IDisposable OnChange(Action<T, string> listener)
    {
        return new DummyDisposable();
    }

    public T CurrentValue { get; }

    private class DummyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}

