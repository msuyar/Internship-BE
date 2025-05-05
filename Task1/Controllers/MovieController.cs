using LMS.Data;
using LMS.Data.Dtos;
using LMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AspNETWebAPIDersleri.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly LMSDBContext context;

    public MovieController(LMSDBContext context)
    {
        //context = new LMSDBContext();
        this.context = context;
    }

    [HttpGet]
    public List<Movie> Get()
    {
        return context.Movies.ToList();
    }

    [HttpGet("{id}")]
    public Movie Get(int id)
    {
        return context.Movies.Find(id);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateMovieDto dto)
    {
        if (dto == null) return BadRequest();

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

        context.Movies.Add(movie);
        context.SaveChanges();

        return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
    }

    // PUT api/movie/{id}
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] UpdateMovieDto dto)
    {
        if (dto == null) return BadRequest();

        var existing = context.Movies.Find(id);
        if (existing == null) return NotFound();

        existing.Title = dto.Title;
        existing.Plot = dto.Plot;
        existing.Cast = dto.Cast;
        existing.Director = dto.Director;
        existing.Category = dto.Category;
        existing.Duration = dto.Duration;
        existing.ReleaseDate = dto.ReleaseDate;

        context.SaveChanges();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        var movie = context.Movies.Find(id);
        context.Movies.Remove(movie);
        context.SaveChanges();
    }
}