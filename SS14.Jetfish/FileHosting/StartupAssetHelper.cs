using Serilog;
using SS14.ConfigProvider.Model;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting.Services;

namespace SS14.Jetfish.FileHosting;

public static class StartupAssetHelper
{
    private const string DefaultProfilePictureDbIdentifier = "DefaultProfilePicture_";

    private static string GetDbIdentifier(string key)
    {
        return DefaultProfilePictureDbIdentifier + key;
    }

    public static readonly string[] ValidTypes =
    [
        "image/png",
        "image/jpeg",
        "image/webp"
    ];

    /// <summary>
    /// Performs first time asset setups
    /// </summary>
    public static async Task FirstTimeAssetSetup(IApplicationBuilder builder)
    {
        using var scope       = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await using var ctx   = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var config            = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var fileService       = scope.ServiceProvider.GetRequiredService<FileService>();

        var userConfig = new UserConfiguration();
        config.Bind(UserConfiguration.Name, userConfig);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SS14.Jetfish (https://github.com/space-wizards/SS14.Jetfish)");

        foreach (var defaultProfilePicture in userConfig.DefaultProfilePictures)
        {
            if (ctx.ConfigurationStore.Any(x => x.Name == GetDbIdentifier(defaultProfilePicture.Key)))
                continue;

            Log.Information("Downloading default profile picture {Name} - {Url}", defaultProfilePicture.Key, defaultProfilePicture.Value);

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

            Log.Information("Downloaded default profile picture: {Name} - {Url}", defaultProfilePicture.Key, uri);
        }

        // TODO: Maybe delete old default profile pictures?
    }
}