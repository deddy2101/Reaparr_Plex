namespace Reaparr.Domain.UnitTests;

public class StringExtensionsToSearchTermsUnitTests : BaseUnitTest
{
    [Test]
    [Arguments("the amazing spiderman", new[] { "amazing", "spiderman" })]
    [Arguments("The Amazing Spider-Man", new[] { "amazing", "spiderman" })]
    [Arguments("spider-man 2", new[] { "spiderman", "2" })]
    [Arguments("Ocean's Eleven", new[] { "oceans", "eleven" })]
    [Arguments("Perché", new[] { "perche" })]
    [Arguments("x-men", new[] { "xmen" })]
    [Arguments("the batman", new[] { "batman" })]
    public void ShouldNormalizeLikeStoredTitles_WhenQueryContainsPunctuationOrStopWords(string query, string[] expected)
    {
        // Act
        var terms = query.ToSearchTerms();

        // Assert
        terms.ShouldBe(expected);
    }

    [Test]
    public void ShouldKeepStopWords_WhenQueryContainsOnlyStopWords()
    {
        // Act
        var terms = "The".ToSearchTerms();

        // Assert
        terms.ShouldBe(["the"]);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    [Arguments(null)]
    public void ShouldReturnNoTerms_WhenQueryIsEmpty(string? query)
    {
        query.ToSearchTerms().ShouldBeEmpty();
    }

    [Test]
    [Arguments("The Amazing Spider-Man")]
    [Arguments("Spider-Man: Homecoming")]
    [Arguments("Ocean's Eleven")]
    [Arguments("Batman & Robin")]
    public void ShouldMatchStoredSearchTitle_WhenQueryIsTheOriginalTitle(string title)
    {
        // Arrange
        var stored = title.ToSearchTitle();

        // Act
        var terms = title.ToSearchTerms();

        // Assert
        terms.ShouldNotBeEmpty();
        terms.ShouldAllBe(term => stored.Contains(term));
    }
}
