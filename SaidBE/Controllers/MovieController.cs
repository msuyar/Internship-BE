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
    public async Task<IActionResult> Get(Guid id)
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
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateMovieDto dto)
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
    public async Task<IActionResult> Delete(Guid id)
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
    
    [HttpPost("filter2")]
    public async Task<IActionResult> GetFilteredMovies2([FromBody] FilterMovieDto filter)
    {
        if (filter.Page <= 0 || filter.PageSize <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Page number and page size must be greater than 0."
            });
        }

        var query = _repository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Title), m => m.Title.Contains(filter.Title))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Plot), m => m.Plot.Contains(filter.Plot))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Cast), m => m.Cast.Contains(filter.Cast))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Director), m => m.Director.Contains(filter.Director))

            // Duration, rating, release year
            .WhereIf(filter.MinDuration.HasValue, m => m.Duration >= filter.MinDuration.Value)
            .WhereIf(filter.MaxDuration.HasValue, m => m.Duration <= filter.MaxDuration.Value)
            .WhereIf(filter.Rating.HasValue, m => m.Rating >= filter.Rating.Value)
            .WhereIf(filter.ReleaseDate.HasValue, m => m.ReleaseDate.Year == filter.ReleaseDate.Value.Year);

        // Category filter (manual since it's multi-value split)
        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            var filterGenres = filter.Category
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(g => g.Trim())
                .ToList();

            query = query.Where(m =>
                !string.IsNullOrWhiteSpace(m.Category) &&
                m.Category.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(g => g.Trim())
                         .Any(g => filterGenres.Contains(g)));
        }

        // Sorting
        query = (filter.SortBy, filter.SortOrder) switch
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

        var movies = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
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
            page = filter.Page,
            pageSize = filter.PageSize,
            data = movies
        });
    }

    [HttpGet("movieDetails/{id:guid}")]
    public async Task<IActionResult> GetMovieDetails(Guid id)
    {
        try
        {
            var movie = await _repository
                .GetAll()                      
                .Include(m => m.Reviews)      
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "There is no movie with this id."
                });
            }
        
            var reviewDtos = movie.Reviews
                .Select(r => new ReviewDto {
                    Id        = r.Id,
                    Rating    = r.Rating,
                    Note      = r.Note,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                
                })
                .ToList();
        
            var movieDetails = new MovieDetailsDto
            {
                Id            = movie.Id,
                Title         = movie.Title,
                Plot          = movie.Plot,
                Cast          = movie.Cast,
                Director      = movie.Director,
                Category      = movie.Category,
                Duration      = movie.Duration,
                ReleaseDate   = movie.ReleaseDate,
                AverageRating = movie.Rating,
                ReviewDtos    = reviewDtos
            };

            return Ok(new
            {
                success = true,
                message = $"Retrieved {reviewDtos.Count} review(s) successfully",
                data    = movieDetails
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}