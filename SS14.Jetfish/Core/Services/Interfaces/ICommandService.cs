using SS14.Jetfish.Core.Commands;

namespace SS14.Jetfish.Core.Services.Interfaces;

public interface ICommandService
{
    Task<ICommand<T>?> Run<T>(ICommand<T> command)  where T : class;
}