namespace LMS.Data.Dtos;

public class AddMovieToWatchlistDto
{
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
}