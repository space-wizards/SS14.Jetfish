using System.Security.Claims;
using SS14.Jetfish.FileHosting.Repositories;
using SS14.Jetfish.FileHosting.Services;

namespace SS14.Jetfish.FileHosting;

public static class FileHostingExtension
{
    public static void AddFileHosting(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<FileService>();
        builder.Services.AddScoped<FileRepository>();
    }
    
    public static void UseFileHosting(this WebApplication app)
    {
        app.MapGet("/project-file/{projectId:guid}/file/{fileId:guid}",
            async (Guid fileId, Guid projectId, FileService fileService, ClaimsPrincipal user) =>
                await fileService.GetProjectFileAsResult(user, projectId, fileId));
                
        app.MapGet("/user-file/{fileId:guid}", async (Guid fileId, FileService fileService, ClaimsPrincipal user) => 
            await fileService.GetUserFileAsResult(user, fileId));
    }
}