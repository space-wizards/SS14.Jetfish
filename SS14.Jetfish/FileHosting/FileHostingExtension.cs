using System.Security.Claims;
using Microsoft.Extensions.FileProviders;
using SS14.Jetfish.FileHosting.Services;

namespace SS14.Jetfish.FileHosting;

public static class FileHostingExtension
{
    public static void UseFileHosting(this WebApplication app)
    {
        app.Map("/files", files =>
        {
            files.UseCors();
            files.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/{fileId:guid}", async (Guid fileId, FileService fileService, ClaimsPrincipal user) => 
                    await fileService.GetFileAsResult(user, fileId));
            });
        });
    }
}