namespace Reaparr.Application.Contracts;

public record PlexMediaDTO : PlexMediaSlimDTO
{
    public required bool HasArt { get; set; }

    public required bool HasTheme { get; set; }

    public required string Studio { get; set; }

    public required string Summary { get; set; }

    public required string? ContentRating { get; set; }

    public required double Rating { get; set; }

    public required DateTime? OriginallyAvailableAt { get; set; }

    public required int TvShowId { get; set; }

    public required int TvShowSeasonId { get; set; }

    public required List<PlexMediaDataDTO> MediaData { get; set; } = [];

    public required List<PlexMediaDTO> Children { get; set; } = [];

    /// <summary>
    /// The season number (for seasons) or episode number within its season (for episodes). Zero for other media types.
    /// Not <c>required</c> so existing constructions keep compiling.
    /// </summary>
    public int IndexNumber { get; set; }
}
