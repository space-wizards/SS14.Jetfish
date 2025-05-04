namespace SS14.Jetfish.Configuration;

/// <summary>
/// Contains configuration options for user related settings.
/// </summary>
public class UserConfiguration
{
    public const string Name = "User";

    /// <summary>
    /// Default profile pictures a user can select from. These will be downloaded only once on first time setup.
    /// Key: name
    /// Value: url
    /// </summary>
    public Dictionary<string, string> DefaultProfilePictures { get; set; } = null!;
}
