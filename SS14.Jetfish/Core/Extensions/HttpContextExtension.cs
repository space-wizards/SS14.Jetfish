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
}
