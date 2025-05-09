using LMS.Data;
using LMS.Data.Dtos;
using LMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

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
        var movies = await _repository.GetAll().ToListAsync();

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
        // For Filtering look at page 181 of the book
        var (found, movie) = await _repository.GetByIdAsync(id);

        if (!found)
        {
            return NotFound(new
            {
                success = false,
                message = $"Movie with ID {id} was not found"
            });
        }

        var movieDto = new MovieDto
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
        await _repository.SaveChangesAsync();

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
        var (found, existing) = await _repository.GetByIdAsync(id);

        if (!found)
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

        await _repository.SaveChangesAsync();

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
        var (found, existing) = await _repository.GetByIdAsync(id);

        if (!found)
        {
            return NotFound(new
            {
                success = false,
                message = $"Movie with ID {id} was not found"
            });
        }

        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            message = $"Movie with ID {id} was deleted successfully"
        });
    }
    /*
     Saved for later on use 
     await _context.Movies
    .AsNoTracking() // ensures no tracking
    .Where(m => m.Category == "Draft")
    .ExecuteDeleteAsync();
     */
    
    [HttpGet("filter-basic")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> FilterByTitleAndDuration(
        string? title = null,
        int? minDuration = null,
        int? maxDuration = null)
    {
        if ((minDuration.HasValue && minDuration < 0) ||
            (maxDuration.HasValue && maxDuration < 0) ||
            (minDuration.HasValue && maxDuration.HasValue && minDuration > maxDuration))
        {
            return BadRequest(new
            {
                success = false,
                message = $"Both durations must be greater than or equal to 0."   
            });
        }
        var query = _repository.GetAll();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(filteredMovie => filteredMovie.Title.Contains(title));

        if (minDuration.HasValue)
        {
            query = query.Where(filteredMovie => filteredMovie.Duration >= minDuration.Value);
        }

        if (maxDuration.HasValue)
        {
            query = query.Where(filteredMovie => filteredMovie.Duration <= maxDuration.Value);
        }

        var movies = await query
            .Select(filteredMovie => new MovieDto
            {
                Id = filteredMovie.Id,
                Title = filteredMovie.Title,
                Plot = filteredMovie.Plot,
                Cast = filteredMovie.Cast,
                Director = filteredMovie.Director,
                Category = filteredMovie.Category,
                Duration = filteredMovie.Duration,
                ReleaseDate = filteredMovie.ReleaseDate,
                Rating = filteredMovie.Rating
            })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            count = movies.Count,
            data = movies
        });
    }

    [HttpGet("filter-date")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> FilterByReleaseDate(int releaseDate)
    {
        if (releaseDate < 1888 || releaseDate > DateTime.Now.Year + 1)
        {
            return BadRequest(new
                {
                    success = false,
                    message = $"The release date must be a valid year in movie history."   
                });
        }
        var movies = await _repository.GetAll()
            .Where(filteredMovie => filteredMovie.ReleaseDate.Year == releaseDate)
            .Select(filteredMovie => new MovieDto
            {
                Id = filteredMovie.Id,
                Title = filteredMovie.Title,
                Plot = filteredMovie.Plot,
                Cast = filteredMovie.Cast,
                Director = filteredMovie.Director,
                Category = filteredMovie.Category,
                Duration = filteredMovie.Duration,
                ReleaseDate = filteredMovie.ReleaseDate,
                Rating = filteredMovie.Rating
            })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            count = movies.Count,
            data = movies
        });
    }
    
    [HttpGet("filter-date-between-years")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> FilterByReleaseYearRange(
        int? minYear,
        int? maxYear)
    {
        // Default min/max if not provided
        minYear ??= 1888;
        maxYear ??= DateTime.Now.Year;

        if (minYear > maxYear || minYear < 1888 || maxYear > DateTime.Now.Year + 1)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid year range. Please check minYear and maxYear values."
            });
        }

        var movies = await _repository.GetAll()
            .Where(filteredMovie => filteredMovie.ReleaseDate.Year >= minYear && filteredMovie.ReleaseDate.Year <= maxYear)
            .Select(filteredMovie => new MovieDto
            {
                Id = filteredMovie.Id,
                Title = filteredMovie.Title,
                Plot = filteredMovie.Plot,
                Cast = filteredMovie.Cast,
                Director = filteredMovie.Director,
                Category = filteredMovie.Category,
                Duration = filteredMovie.Duration,
                ReleaseDate = filteredMovie.ReleaseDate,
                Rating = filteredMovie.Rating
            })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            range = new { minYear, maxYear },
            count = movies.Count,
            data = movies
        });
    }
}