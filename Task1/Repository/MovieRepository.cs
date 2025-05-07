using LMS.Data;
using LMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class MovieRepository : IMovieRepository
{
    private readonly LMSDBContext _context;

    public MovieRepository(LMSDBContext context)
    {
        _context = context;
    }

    public IQueryable<Movie> GetAll()
    {
        return _context.Movies.AsQueryable();
    }

    public async Task<(bool found, Movie? movie)> GetByIdAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        return (movie != null, movie);
    }

    public async Task AddAsync(Movie movie)
    {
        await _context.Movies.AddAsync(movie);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
    }
}