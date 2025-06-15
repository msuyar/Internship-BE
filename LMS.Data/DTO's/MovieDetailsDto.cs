namespace LMS.Data.Dtos;

public class MovieDetailsDto
{
    public string Title { get; set; }
    public double AverageRating { get; set; }
    public List<ReviewDto> ReviewDtos { get; set; }
    
}