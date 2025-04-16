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
            files.UseRouting();
            files.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/project/{projectId:guid}/file/{fileId:guid}",
                    async (Guid fileId, Guid projectId, FileService fileService, ClaimsPrincipal user) =>
                        await fileService.GetProjectFileAsResult(user, projectId, fileId));
                
                endpoints.MapGet("/{fileId:guid}", async (Guid fileId, FileService fileService, ClaimsPrincipal user) => 
                    await fileService.GetUserFileAsResult(user, fileId));
            });
        });
    }
}