using System.Globalization;

namespace SS14.Jetfish.Configuration;

public class ServerConfiguration
{
    public const string Name = "Server";

    public Uri Host { get; set; } = new("https://localhost:7154");
    public List<string>? CorsOrigins { get; set; }
    public CultureInfo Language { get; set; } = null!; //new("en-US");

    /// <summary>
    /// Enables https redirection if true. Set this to false if run behind a reverse proxy
    /// </summary>
    public bool UseHttps { get; set; } = false;

    /// <summary>
    /// Enables support for reverse proxy headers like "X-Forwarded-Host" if true. Set this to true if run behind a reverse proxy
    /// </summary>
    public bool UseForwardedHeaders { get; set; } = true;

    /// <summary>
    /// Sets the request base path used before any routes apply i.e. "/base/api/Maps" with "/base" being the PathBase. <br/>
    /// Set this if run behind a reverse proxy on a sub path and the proxy doesn't strip the path the server is hosted on.
    /// </summary>
    /// <remarks>
    /// Add a slash before the path: "/path"
    /// </remarks>
    public string? PathBase { get; set; }

    public bool EnableMigrations { get; set; } = true;

    /// <summary>
    /// A claim that is required to be present before allowing access to the service.
    /// Any authenticated user has access if this is null.
    /// </summary>
    public string? RequiredClaim { get; set; }

    /// <summary>
    /// If <see cref="RequiredClaim"/> is not null a user is only allowed to access the service if the required claim
    /// matches any of the configured values.
    /// If this is null any value is valid.
    /// </summary>
    public List<string>? RequiredClaimValues { get; set; }

    /// <summary>
    /// A claim that gives a person who has it full access. Recommended for first setup.
    /// </summary>
    public string? AdminClaim { get; set; }

    /// <summary>
    /// The directory where user uploaded files are stored.
    /// </summary>
    public string UserContentDirectory { get; set; } = "data/uploads";
}