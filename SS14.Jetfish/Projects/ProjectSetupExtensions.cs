using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Projects.Commands.Handlers;
using SS14.Jetfish.Projects.Hubs;
using SS14.Jetfish.Projects.Repositories;

namespace SS14.Jetfish.Projects;

public static class ProjectSetupExtensions
{
    public static void AddProjects(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ProjectHub>();

        builder.Services.AddScoped<ProjectRepository>();
        builder.Services.AddScoped<ICommandHandler, CreateProjectCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, DeleteProjectCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, UpdateProjectCommandHandler>();
    }
}
