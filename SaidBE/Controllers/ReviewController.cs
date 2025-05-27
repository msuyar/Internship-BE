using AspNETWebAPIDersleri.Repository;
using LMS.Data.Dtos;
using LMS.Data.Entities;
using LMS.Data.Enums;
using LMS.Data.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace AspNETWebAPIDersleri.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _reviewRepository.GetAllReviewsAsync();

        var reviewDtos = reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserFullName = r.User.FirstName + " " + r.User.LastName,
            MovieId = r.MovieId,
            MovieTitle = r.Movie.Title,
            Rating = r.Rating,
            Note = r.Note,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();

        return Ok(new
        {
            success = true,
            message = $"Retrieved {reviewDtos.Count()} movie(s) successfully",
            data = reviewDtos
        });;
    }

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
    {
        // Validate user and movie existence
        var userExists = await _reviewRepository.UserExistsAsync(dto.UserId);
        var movieExists = await _reviewRepository.MovieExistsAsync(dto.MovieId);

        if (!userExists || !movieExists)
        {
            return BadRequest(new { message = "Invalid user or movie ID." });
        }

        // Check if user already reviewed this movie
        var alreadyReviewed = await _reviewRepository.HasUserReviewedMovieAsync(dto.UserId, dto.MovieId);
        if (alreadyReviewed)
        {
            return BadRequest(new { message = "You have already reviewed this movie." });
        }

        // Create and save new review
        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            MovieId = dto.MovieId,
            Rating = dto.Rating,
            Note = dto.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddReviewAsync(review);

        return Ok(new { message = "Review submitted successfully." });
    }
    
    [HttpPost("edit")]
    public async Task<IActionResult> EditReview([FromBody] CreateReviewDto dto)
    {
        // Validate user and movie existence
        var userExists = await _reviewRepository.UserExistsAsync(dto.UserId);
        var movieExists = await _reviewRepository.MovieExistsAsync(dto.MovieId);

        if (!userExists || !movieExists)
        {
            return BadRequest(new { message = "Invalid user or movie ID." });
        }

        // Check if user already reviewed this movie
        var review = await _reviewRepository.GetUserReviewAsync(dto.UserId, dto.MovieId);
        if (review == null)
        {
            return BadRequest(new { message = "No such review exists." });
        }

        // Create and save new review
        review.Rating = dto.Rating;
        review.Note = dto.Note;
        review.UpdatedAt = DateTime.UtcNow;

        await _reviewRepository.UpdateReviewAsync(review);

        return Ok(new { message = "Review updated successfully." });
    }
}