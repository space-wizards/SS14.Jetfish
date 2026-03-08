using System.Windows.Input;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Projects.Commands;
using SS14.Jetfish.Projects.Commands.Handlers;
using SS14.Jetfish.Projects.Repositories;

namespace SS14.Jetfish.Projects;

public static class ProjectSetupExtensions
{
    public static void AddProjects(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ProjectRepository>();
        builder.Services.AddScoped<CommentRepository>();
        builder.Services.AddScoped<ICommandHandler, CreateOrUpdateCommentHandler>();
        builder.Services.AddScoped<ICommandHandler, CreateProjectCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, DeleteProjectCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, UpdateProjectCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, DeleteCommentCommandHandler>();
    }
}
