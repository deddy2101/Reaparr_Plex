using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Reaparr.Application.Contracts;
using Reaparr.Settings.Contracts;

namespace Reaparr.AppHost;

/// <summary>
/// Authenticates requests to the internal API that carry a valid <see cref="HeaderName"/> header as the Admin user.
/// Intended for trusted external clients such as the Jellyfin plugin.
/// </summary>
public class ApiKeyAuthenticationMiddleware
{
    /// <summary>
    /// The request header carrying the API key.
    /// </summary>
    public const string HeaderName = "X-Api-Key";
    /// <summary>
    /// The authentication type set on the identity created for a valid API key.
    /// </summary>
    public const string AuthenticationType = "ApiKey";

    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _log;
    private readonly IAuthenticationSettings _authenticationSettings;

    /// <summary>
    /// Constructor for ApiKeyAuthenticationMiddleware
    /// </summary>
    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        Serilog.ILogger log,
        IAuthenticationSettings authenticationSettings
    )
    {
        _next = next;
        _log = log.ForContext<ApiKeyAuthenticationMiddleware>();
        _authenticationSettings = authenticationSettings;
    }

    /// <summary>
    /// API key authentication middleware invocation
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/>.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;

        // The public API (Torznab / qBittorrent emulation) has its own per-integration keys.
        var isInternalApi =
            path.StartsWithSegments(ApiRoutes.Base) && !path.StartsWithSegments(ApiRoutes.Base + "/public");

        if (!isInternalApi || !context.Request.Headers.TryGetValue(HeaderName, out var headerValues))
        {
            await _next(context);
            return;
        }

        var settings = _authenticationSettings.ApiKeyAuthentication;
        var provided = headerValues.ToString();

        if (settings.IsUsable() && KeysMatch(provided, settings.ApiKey))
        {
            context.User = CreateAdminPrincipal();
        }
        else
        {
            _log.Here()
                .Warning(
                    "Rejected {HeaderName} authentication for {Path} from {RemoteIp}. Enabled: {Enabled}",
                    HeaderName,
                    path.Value,
                    context.Connection.RemoteIpAddress,
                    settings.Enabled
                );
        }

        await _next(context);
    }

    private static bool KeysMatch(string provided, string expected)
    {
        // Hash both so the comparison is fixed-length and constant-time regardless of the provided length.
        var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
        return CryptographicOperations.FixedTimeEquals(providedHash, expectedHash);
    }

    private static ClaimsPrincipal CreateAdminPrincipal()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "api-key-client"),
            new Claim(ClaimTypes.NameIdentifier, "api-key-client"),
            new Claim(ClaimTypes.Role, DefaultUserAppCredentials.DefaultAdminRole),
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationType));
    }
}
