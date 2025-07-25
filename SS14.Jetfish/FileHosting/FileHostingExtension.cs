﻿using System.Reflection.Emit;
using System.Security.Claims;
using SS14.Jetfish.FileHosting.Model;
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
            async (Guid fileId, Guid projectId, string? label, FileService fileService, ClaimsPrincipal user) =>
                await fileService.GetProjectFileAsResult(user, projectId, fileId, label));

        app.MapGet("/user-file/{fileId:guid}", async (Guid fileId, string? label, FileService fileService, ClaimsPrincipal user) =>
            await fileService.GetUserFileAsResult(user, fileId, label));

        app.MapGet("/global-file/{fileId:guid}", async (Guid fileId, string? label, FileService fileService) =>
            await fileService.GetGlobalFileAsResult(fileId, label));
    }

    public static string FileTypeUrl(this FileType type) => type switch
    {
        FileType.Global => "global-file",
        FileType.User => "user-file",
        FileType.Project => "project-file",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
