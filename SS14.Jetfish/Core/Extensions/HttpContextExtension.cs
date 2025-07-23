using Serilog.Events;

namespace SS14.Jetfish.Core.Extensions;

public static class HttpContextExtension
{
    public static async Task WriteAccessDeniedResponse(
        this HttpContext context,
        string message = "Access Denied",
        CancellationToken ct = new())
    {
        context.Response.Clear();
        await context.Response.WriteAsync(message, ct);
        context.Response.StatusCode = 403;
        await context.Response.CompleteAsync();
    }

    public static LogEventLevel GetRequestLogLevel(HttpContext httpContext, double d, Exception? exception)
    {
        if (exception != null || httpContext.Response.StatusCode > 499)
            return LogEventLevel.Error;

        return LogEventLevel.Verbose;
    }
}
