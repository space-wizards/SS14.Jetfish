using SS14.Jetfish.Core.Commands;

namespace SS14.Jetfish.Core.Services.Interfaces;

public interface ICommandHandler
{
    string CommandName { get; }
    Task<ICommand<T>?> Handle<T>(ICommand<T> command)  where T : class;
}