namespace SS14.Jetfish.Core.Commands;

public interface ICommand<T>  where T : class
{
    string Name { get; }
    
    T? Result { get; set; }
}