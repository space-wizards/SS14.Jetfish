using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Projects.Commands.Handlers;
using SS14.Jetfish.Projects.Repositories;

namespace SS14.Jetfish.Projects;

public static class ProjectSetupExtensions
{
    public static void AddProjects(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ProjectRepository>();
        builder.Services.AddScoped<ICommandHandler, CreateOrUpdateProjectCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, CreateProjectCommandHandler>();
        builder.Services.AddScoped<ICommandHandler, DeleteProjectCommandHandler>();
    }
}