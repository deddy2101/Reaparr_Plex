namespace Reaparr.Application.UnitTests;

public class PlexMediaDTOMapperIndexNumberUnitTests : BaseUnitTest
{
    [Test]
    public void ShouldMapEpisodeNumberToIndexNumber_WhenMappingAnEpisode()
    {
        // Arrange
        var episode = FakeData.GetPlexTvShowEpisode(new Seed(2026)).Generate();
        episode.EpisodeNumber = 7;

        // Act
        var dto = episode.ToDTO();

        // Assert
        dto.IndexNumber.ShouldBe(7);
    }

    [Test]
    public void ShouldMapSeasonNumberToIndexNumber_WhenMappingASeason()
    {
        // Arrange
        var season = FakeData.GetPlexTvShowSeason(new Seed(2026)).Generate();
        season.SeasonNumber = 3;

        // Act
        var dto = season.ToDTO();

        // Assert
        dto.IndexNumber.ShouldBe(3);
        dto.Children.Count.ShouldBe(season.Episodes.Count);
    }
}
