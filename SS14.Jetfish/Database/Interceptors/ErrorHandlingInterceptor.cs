using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SS14.Jetfish.Database.Interceptors;

public class ErrorHandlingInterceptor : DbCommandInterceptor
{
    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
    }

    public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}