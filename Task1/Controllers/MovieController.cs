using LMS.Data;
using LMS.Data.Dtos;
using LMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AspNETWebAPIDersleri.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly LMSDBContext _context;
    private readonly IMovieRepository _repository;

    public MovieController(IMovieRepository repository, LMSDBContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
        // I made this so the code would be more readable 
        // if you want I can turn this into an oneliner
        var movies = _context.Movies.ToList();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var movie = _context.Movies.Find(id);

        if (movie == null)
        {
            return NotFound();
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

        return Ok(movieDto);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateMovieDto dto)
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

        _context.Movies.Add(movie);
        _context.SaveChanges();

        return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
    }

    // PUT api/movie/{id}
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] UpdateMovieDto dto)
    {
        var existing = _context.Movies.Find(id);
        
        if (existing == null)
        {
            return NotFound();
        }

        existing.Title = dto.Title;
        existing.Plot = dto.Plot;
        existing.Cast = dto.Cast;
        existing.Director = dto.Director;
        existing.Category = dto.Category;
        existing.Duration = dto.Duration;
        existing.ReleaseDate = dto.ReleaseDate;

        _context.SaveChanges();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var movie = _context.Movies.Find(id);
        
        if (movie == null)
        {
            return NotFound();
        }
        
        _context.Movies.Remove(movie);
        _context.SaveChanges();

        return Ok();
    }
}