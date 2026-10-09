namespace Reaparr.Settings.Contracts;

/// <summary>
/// Configuration for API key authentication of the internal API (used by external clients such as the Jellyfin plugin).
/// </summary>
public interface IApiKeyAuthenticationSettings
{
    /// <summary>
    /// Whether API key authentication is enabled
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// The secret expected in the X-Api-Key request header. Must be at least <see cref="ApiKeyAuthenticationSettings.MinimumKeyLength"/> characters.
    /// </summary>
    string ApiKey { get; set; }
}
