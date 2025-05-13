using LMS.Data;
using LMS.Data.Dtos;
using LMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using LMS.Data.Enums;
using System.Linq; 
using System.Linq.Expressions; 

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
        var movie = await _repository.GetByIdAsync(id);

        if (movie == null)
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

        return Ok(new
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
    
    [HttpGet("filter-date-between-years")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> FilterByReleaseYearRange(
        int? minYear,
        int? maxYear)
    {

        if ((!minYear.HasValue && !maxYear.HasValue) || minYear > maxYear)
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
    
    [HttpGet("filter-sort-paginate")]
    public async Task<IActionResult> GetFilteredSortedPaginatedMovies(
        string? title = null,
        int? minDuration = null,
        int? maxDuration = null,
        string? sortOrder = "asc",
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Page number and page size must be greater than 0."
            });
        }

        var query = _repository.GetAll();

        // Filtering
        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(m => m.Title.Contains(title));

        if (minDuration.HasValue)
            query = query.Where(m => m.Duration >= minDuration.Value);

        if (maxDuration.HasValue)
            query = query.Where(m => m.Duration <= maxDuration.Value);

        // Sorting
        query = sortOrder?.ToLower() == "desc"
            ? query.OrderByDescending(m => m.Title)
            : query.OrderBy(m => m.Title);

        // Pagination
        var totalItems = await query.CountAsync();
        var movies = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                Plot = m.Plot,
                Cast = m.Cast,
                Director = m.Director,
                Category = m.Category,
                Duration = m.Duration,
                ReleaseDate = m.ReleaseDate,
                Rating = m.Rating
            })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            totalItems,
            pageNumber,
            pageSize,
            data = movies
        });
    }
    
     [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredMovies(
        string? title = null,
        string? plot = null,
        string? cast = null,
        string? director = null,
        string? category = null,
        int? minDuration = null,
        int? maxDuration = null,
        float? rating = null,
        int? releaseYear = null,
        MovieSortBy sortBy = MovieSortBy.Title,
        SortingType sortOrder = SortingType.asc,
        int page = 1,
        int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Page number and page size must be greater than 0."
            });
        }

        var query = _repository.GetAll();

        // Filtering
        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(m => m.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(plot))
        {
            query = query.Where(m => m.Plot.Contains(plot));
        }

        if (!string.IsNullOrWhiteSpace(cast))
        {
            query = query.Where(m => m.Cast.Contains(cast));
        }

        if (!string.IsNullOrWhiteSpace(director))
        {
            query = query.Where(m => m.Director.Contains(director));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var filterGenres = category
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(g => g.Trim())
                .ToList();

            query = query.Where(m =>
                !string.IsNullOrWhiteSpace(m.Category) &&
                m.Category.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim())
                    .Any(g => filterGenres.Contains(g)));
        }

        if (minDuration.HasValue)
        {
            query = query.Where(m => m.Duration >= minDuration.Value);
        }

        if (maxDuration.HasValue)
        {
            query = query.Where(m => m.Duration <= maxDuration.Value);
        }

        if (rating.HasValue)
        {
            query = query.Where(m => m.Rating >= rating.Value);
        }

        if (releaseYear.HasValue)
        {
            query = query.Where(m => m.ReleaseDate.Year == releaseYear.Value);
        }

        // Sorting
        query = (sortBy, sortOrder) switch
        {
            (MovieSortBy.Rating, SortingType.desc) => query.OrderByDescending(m => m.Rating),
            (MovieSortBy.Rating, _) => query.OrderBy(m => m.Rating),

            (MovieSortBy.Duration, SortingType.desc) => query.OrderByDescending(m => m.Duration),
            (MovieSortBy.Duration, _) => query.OrderBy(m => m.Duration),

            (MovieSortBy.ReleaseDate, SortingType.desc) => query.OrderByDescending(m => m.ReleaseDate),
            (MovieSortBy.ReleaseDate, _) => query.OrderBy(m => m.ReleaseDate),

            (MovieSortBy.Title, SortingType.desc) => query.OrderByDescending(m => m.Title),
            _ => query.OrderBy(m => m.Title)
        };

        // Pagination
        var totalItems = await query.CountAsync();
        if (totalItems == 0)
        {
            return Ok(new
            {
                success = true,
                totalItems = 0,
                message = "No movies found with the specified filters.",
                data = Array.Empty<MovieDto>()
            });
        }
        var movies = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                Plot = m.Plot,
                Cast = m.Cast,
                Director = m.Director,
                Category = m.Category,
                Duration = m.Duration,
                ReleaseDate = m.ReleaseDate,
                Rating = m.Rating
            })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            totalItems,
            page,
            pageSize,
            data = movies
        });
    }
}