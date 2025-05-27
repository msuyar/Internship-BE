using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Dtos;

public class CreateReviewDto
{
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }

    [Required]
    [Range(1, 10)]
    public double Rating { get; set; }

    public string? Note { get; set; }
}