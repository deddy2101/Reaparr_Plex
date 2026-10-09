namespace Reaparr.Settings.Contracts;

/// <summary>
/// Configuration for API key authentication of the internal API.
/// <para> Intentionally only configurable through ReaparrSettings.json, it is not exposed to the web UI. </para>
/// </summary>
public record ApiKeyAuthenticationSettings
    : BaseSettingsModule<ApiKeyAuthenticationSettings>,
        IBaseSettingsModule<ApiKeyAuthenticationSettings>,
        IApiKeyAuthenticationSettings
{
    public const int MinimumKeyLength = 32;

    // Not `required`: a partially filled hand-edited ReaparrSettings.json must not fail deserialization,
    // since that falls back to default settings for the whole file.

    /// <inheritdoc />
    public bool Enabled
    {
        get;
        set => SetProperty(ref field, value);
    }

    /// <inheritdoc />
    public string ApiKey
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public static ApiKeyAuthenticationSettings Create() => new();

    /// <summary>
    /// The key is only usable when enabled and long enough to resist guessing.
    /// </summary>
    public bool IsUsable() => Enabled && ApiKey.Length >= MinimumKeyLength;
}
