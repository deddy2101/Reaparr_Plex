using Autofac;
using Reaparr.Application.Contracts;
using Reaparr.Settings.Contracts;

namespace Reaparr.AppHost.UnitTests;

public class ApiKeyAuthenticationMiddlewareUnitTests : BaseUnitTest<ApiKeyAuthenticationMiddleware>
{
    private static readonly string ValidKey = new('k', ApiKeyAuthenticationSettings.MinimumKeyLength);

    [Test]
    public async Task ShouldAuthenticateAsAdmin_WhenKeyMatchesAndEnabled()
    {
        // Arrange
        var context = CreateHttpContext("/api/PlexLibrary", ValidKey);
        SetupSettings(enabled: true, apiKey: ValidKey);

        // Act
        var nextCalled = await Invoke(context);

        // Assert
        nextCalled.ShouldBeTrue();
        context.User.Identity!.IsAuthenticated.ShouldBeTrue();
        context.User.Identity.AuthenticationType.ShouldBe(ApiKeyAuthenticationMiddleware.AuthenticationType);
        context.User.IsInRole(DefaultUserAppCredentials.DefaultAdminRole).ShouldBeTrue();
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenKeyDoesNotMatch()
    {
        // Arrange
        var context = CreateHttpContext("/api/PlexLibrary", new string('x', ApiKeyAuthenticationSettings.MinimumKeyLength));
        SetupSettings(enabled: true, apiKey: ValidKey);

        // Act
        var nextCalled = await Invoke(context);

        // Assert
        nextCalled.ShouldBeTrue();
        context.User.Identity!.IsAuthenticated.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenDisabled()
    {
        // Arrange
        var context = CreateHttpContext("/api/PlexLibrary", ValidKey);
        SetupSettings(enabled: false, apiKey: ValidKey);

        // Act
        var nextCalled = await Invoke(context);

        // Assert
        nextCalled.ShouldBeTrue();
        context.User.Identity!.IsAuthenticated.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenConfiguredKeyIsTooShort()
    {
        // Arrange
        const string shortKey = "short";
        var context = CreateHttpContext("/api/PlexLibrary", shortKey);
        SetupSettings(enabled: true, apiKey: shortKey);

        // Act
        var nextCalled = await Invoke(context);

        // Assert
        nextCalled.ShouldBeTrue();
        context.User.Identity!.IsAuthenticated.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenPathIsPublicApi()
    {
        // Arrange
        var context = CreateHttpContext($"/api/public/integrations/{Guid.NewGuid()}/indexer/api", ValidKey);
        SetupSettings(enabled: true, apiKey: ValidKey);

        // Act
        var nextCalled = await Invoke(context);

        // Assert
        nextCalled.ShouldBeTrue();
        context.User.Identity!.IsAuthenticated.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenPathIsOutsideApi()
    {
        // Arrange
        var context = CreateHttpContext("/progress", ValidKey);
        SetupSettings(enabled: true, apiKey: ValidKey);

        // Act
        var nextCalled = await Invoke(context);

        // Assert
        nextCalled.ShouldBeTrue();
        context.User.Identity!.IsAuthenticated.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldNotAuthenticate_WhenHeaderIsMissing()
    {
        // Arrange
        var context = CreateHttpContext("/api/PlexLibrary", apiKey: null);
        SetupSettings(enabled: true, apiKey: ValidKey);

        // Act
        var nextCalled = await Invoke(context);

        // Assert
        nextCalled.ShouldBeTrue();
        context.User.Identity!.IsAuthenticated.ShouldBeFalse();
    }

    private async Task<bool> Invoke(HttpContext context)
    {
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var sut = Mock.Create<ApiKeyAuthenticationMiddleware>(new TypedParameter(typeof(RequestDelegate), next));
        await sut.InvokeAsync(context);

        return nextCalled;
    }

    private void SetupSettings(bool enabled, string apiKey)
    {
        var apiKeySettings = new ApiKeyAuthenticationSettings { Enabled = enabled, ApiKey = apiKey };
        Mock.Mock<IAuthenticationSettings>().Setup(x => x.ApiKeyAuthentication).Returns(apiKeySettings);
    }

    private static HttpContext CreateHttpContext(string path, string? apiKey)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        if (apiKey is not null)
            context.Request.Headers[ApiKeyAuthenticationMiddleware.HeaderName] = apiKey;

        return context;
    }
}
