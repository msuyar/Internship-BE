namespace LMS.Data.Dtos;

public class ReviewDto
{ 
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; }
    public Guid MovieId { get; set; }
    public string MovieTitle { get; set; }
    public double Rating { get; set; }
    public string Note { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; }
}