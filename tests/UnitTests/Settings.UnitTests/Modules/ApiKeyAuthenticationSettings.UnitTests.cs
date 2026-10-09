using Reaparr.Settings.Contracts;

namespace Reaparr.Settings.UnitTests;

public class ApiKeyAuthenticationSettingsUnitTests : BaseUnitTest
{
    [Test]
    public void ShouldKeepOtherSettings_WhenApiKeySectionIsMissingFromJson()
    {
        // Arrange
        const string json = """{"AuthenticationSettings":{"ResetCredentials":true}}""";

        // Act
        var settings = UserSettingsSerializer.Deserialize(json);

        // Assert
        settings.AuthenticationSettings.ResetCredentials.ShouldBeTrue();
        settings.AuthenticationSettings.ApiKeyAuthentication.Enabled.ShouldBeFalse();
        settings.AuthenticationSettings.ApiKeyAuthentication.ApiKey.ShouldBeEmpty();
    }

    [Test]
    public void ShouldKeepOtherSettings_WhenApiKeySectionIsPartial()
    {
        // Arrange
        const string json =
            """{"AuthenticationSettings":{"ResetCredentials":true,"ApiKeyAuthentication":{"Enabled":true}}}""";

        // Act
        var settings = UserSettingsSerializer.Deserialize(json);

        // Assert
        settings.AuthenticationSettings.ResetCredentials.ShouldBeTrue();
        settings.AuthenticationSettings.ApiKeyAuthentication.Enabled.ShouldBeTrue();
    }

    [Test]
    public void ShouldNotBeUsable_WhenDisabledOrKeyIsTooShort()
    {
        new ApiKeyAuthenticationSettings { Enabled = false, ApiKey = new string('a', 64) }.IsUsable().ShouldBeFalse();
        new ApiKeyAuthenticationSettings { Enabled = true, ApiKey = "short" }.IsUsable().ShouldBeFalse();
        new ApiKeyAuthenticationSettings { Enabled = true, ApiKey = new string('a', 32) }.IsUsable().ShouldBeTrue();
    }
}
