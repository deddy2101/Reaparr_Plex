namespace Reaparr.Settings.Contracts;

public interface IAuthenticationSettings
{
    bool ResetCredentials { get; set; }

    HeaderAuthenticationSettings HeaderAuthentication { get; set; }

    ApiKeyAuthenticationSettings ApiKeyAuthentication { get; set; }
}
