namespace SS14.Jetfish.Core.Commands;

public abstract class BaseCommand<T> : ICommand<T> where T : class
{
    public abstract string Name { get; }
    public T? Result { get; set; }
}