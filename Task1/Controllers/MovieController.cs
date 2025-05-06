using LMS.Data;
using LMS.Data.Dtos;
using LMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AspNETWebAPIDersleri.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieRepository _repository;

    public MovieController(IMovieRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var movies = await _repository.GetAllAsync();

        return Ok(new
        {
            success = true,
            message = $"Retrieved {movies.Count()} movie(s) successfully",
            data = movies
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var movie = await _repository.GetByIdAsync(id);

        if (movie == null)
        {
            return NotFound(new
            {
                success = false,
                message = $"Movie with ID {id} was not found"
            });
        }

        var movieDto = new ReadMovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Plot = movie.Plot,
            Cast = movie.Cast,
            Director = movie.Director,
            Category = movie.Category,
            Duration = movie.Duration,
            ReleaseDate = movie.ReleaseDate,
            Rating = movie.Rating
        };

        return Ok(new
        {
            success = true,
            message = "Movie retrieved successfully",
            data = movieDto
        });
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateMovieDto dto)
    {
        var movie = new Movie
        {
            Title = dto.Title,
            Plot = dto.Plot,
            Cast = dto.Cast,
            Director = dto.Director,
            Category = dto.Category,
            Duration = dto.Duration,
            ReleaseDate = dto.ReleaseDate,
            Rating = dto.Rating
        };

        await _repository.AddAsync(movie);

        return CreatedAtAction(nameof(Get), new { id = movie.Id }, new
        {
            success = true,
            message = "Movie created successfully",
            data = movie
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateMovieDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing == null)
        {
            return NotFound(new
            {
                success = false,
                message = $"Movie with ID {id} was not found"
            });
        }

        existing.Title = dto.Title;
        existing.Plot = dto.Plot;
        existing.Cast = dto.Cast;
        existing.Director = dto.Director;
        existing.Category = dto.Category;
        existing.Duration = dto.Duration;
        existing.ReleaseDate = dto.ReleaseDate;

        await _repository.UpdateAsync(existing);

        return Ok(new
        {
            success = true,
            message = "Movie updated successfully",
            data = existing
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing == null)
        {
            return NotFound(new
            {
                success = false,
                message = $"Movie with ID {id} was not found"
            });
        }

        await _repository.DeleteAsync(id);

        return Ok(new
        {
            success = true,
            message = $"Movie with ID {id} was deleted successfully"
        });
    }
}