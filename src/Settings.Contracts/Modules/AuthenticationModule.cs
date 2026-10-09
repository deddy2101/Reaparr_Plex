namespace Reaparr.Settings.Contracts;

public record AuthenticationModule
    : BaseSettingsModule<AuthenticationModule>,
        IBaseSettingsModule<AuthenticationModule>,
        IAuthenticationSettings
{
    public required bool ResetCredentials
    {
        get;
        set => SetProperty(ref field, value);
    }

    public required HeaderAuthenticationSettings HeaderAuthentication
    {
        get;
        set => SetProperty(ref field, value);
    } = HeaderAuthenticationSettings.Create();

    /// <summary>
    /// Not marked required on purpose: settings files written before this module existed must still deserialize.
    /// </summary>
    public ApiKeyAuthenticationSettings ApiKeyAuthentication
    {
        get;
        set => SetProperty(ref field, value);
    } = ApiKeyAuthenticationSettings.Create();

    /// <summary>
    /// When true, will reset the Reaparr app credentials and then set to false again.
    /// </summary>
    /// <returns></returns>
    public static AuthenticationModule Create() =>
        new()
        {
            ResetCredentials = false,
            HeaderAuthentication = HeaderAuthenticationSettings.Create(),
            ApiKeyAuthentication = ApiKeyAuthenticationSettings.Create(),
        };
}
