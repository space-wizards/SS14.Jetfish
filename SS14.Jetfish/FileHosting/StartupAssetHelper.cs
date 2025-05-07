using Microsoft.Extensions.Options;
using Serilog;
using SS14.ConfigProvider.Model;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting.Services;

namespace SS14.Jetfish.FileHosting;

public class StartupAssetHelper
{
    public StartupAssetHelper()
    {
        throw new InvalidOperationException("This class should not be constructed."); // workaround to get this class into ILogger<StartupAssetHelper>
    }

    private const string DefaultProfilePictureDbIdentifier = "DefaultProfilePicture_";

    public static string GetDbIdentifier(string key)
    {
        return DefaultProfilePictureDbIdentifier + key;
    }

    public static readonly string[] ValidTypes =
    [
        "image/png",
        "image/jpeg",
        "image/webp",
    ];

    /// <summary>
    /// Performs first time asset setups
    /// </summary>
    public static async Task FirstTimeAssetSetup(IApplicationBuilder builder)
    {
        using var scope       = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await using var ctx   = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var options            = scope.ServiceProvider.GetRequiredService<IOptions<UserConfiguration>>();
        var fileService       = scope.ServiceProvider.GetRequiredService<FileService>();
        var logger            = scope.ServiceProvider.GetRequiredService<ILogger<StartupAssetHelper>>();


        var userConfig = options.Value;

        if (userConfig.DefaultProfilePictures.Count == 0)
            throw new InvalidOperationException("There must be at least one default profile picture.");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SS14.Jetfish (https://github.com/space-wizards/SS14.Jetfish)");

        foreach (var defaultProfilePicture in userConfig.DefaultProfilePictures)
        {
            if (ctx.ConfigurationStore.Any(x => x.Name == GetDbIdentifier(defaultProfilePicture.Key)))
                continue;

            logger.LogInformation("Downloading default profile picture {Name} - {Url}", defaultProfilePicture.Key, defaultProfilePicture.Value);

            if (!Uri.TryCreate(defaultProfilePicture.Value, UriKind.Absolute, out var uri))
                throw new UriFormatException($"Invalid profile picture url for {defaultProfilePicture.Key}\n{defaultProfilePicture.Value}");

            var imageRequest = await httpClient.GetAsync(uri);
            imageRequest.EnsureSuccessStatusCode();

            if (!imageRequest.Content.Headers.TryGetValues("Content-Type", out var contentTypes))
                throw new FormatException($"No Content Type header present in profile picture url: {defaultProfilePicture.Key}\n{defaultProfilePicture.Value}");

            var enumerable = contentTypes as string[] ?? contentTypes.ToArray();
            foreach (var contentType in enumerable)
            {
                if (ValidTypes.Contains(contentType))
                    continue;

                throw new FormatException($"Content type {contentType} is not supported!");
            }

            var imageStream = await imageRequest.Content.ReadAsStreamAsync();
            var fileResult = await fileService.UploadGlobalFile(imageStream, uri.Segments[^1], enumerable.First());
            if (!fileResult.IsSuccess)
                throw fileResult.Error;

            await ctx.ConfigurationStore.AddAsync(new ConfigurationStore()
            {
                Name = GetDbIdentifier(defaultProfilePicture.Key),
                Value = fileResult.Value.Id.ToString(),
                UpdatedOn = DateTime.UtcNow
            });
            await ctx.SaveChangesAsync();

            logger.LogInformation("Downloaded default profile picture: {Name} - {Url}", defaultProfilePicture.Key, uri);
        }

        // TODO: Maybe delete old default profile pictures?

        await fileService.DeleteLostFiles();
    }
}
