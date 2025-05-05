using LMS.Data;
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
    public void Post([FromBody] Movie movie)
    {
        context.Movies.Add(movie);
        context.SaveChanges();
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] Movie movie)
    {
        if (movie == null) return BadRequest();

        var existing = context.Movies.Find(id);
        if (existing == null) return NotFound();

        var originalRating = existing.Rating;
        var originalID = existing.Id;

        // Update everything from the incoming object
        context.Entry(existing).CurrentValues.SetValues(movie);

        // Restore the original rating to prevent it from being overwritten
        existing.Rating = originalRating;
        existing.Id = originalID;

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