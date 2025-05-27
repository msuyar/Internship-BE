using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Data.Entities;

namespace AspNETWebAPIDersleri.Repository;

public class ReviewRepository : IReviewRepository
{
    private readonly LMSDBContext _context;

    public ReviewRepository(LMSDBContext context)
    {
        _context = context;
    }

    public async Task<bool> HasUserReviewedMovieAsync(Guid userId, Guid movieId)
    {
        return await _context.Reviews
            .AnyAsync(r => r.UserId == userId && r.MovieId == movieId);
    }

    public async Task AddReviewAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<bool> MovieExistsAsync(Guid movieId)
    {
        return await _context.Movies.AnyAsync(m => m.Id == movieId);
    }
    
    public async Task<Review?> GetUserReviewAsync(Guid userId, Guid movieId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
    }

    public async Task UpdateReviewAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Review>> GetAllReviewsAsync()
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Movie)
            .ToListAsync();
    }
    
    public async Task UpdateMovieAverageRating(Guid movieId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.MovieId == movieId)
            .ToListAsync();

        var movie = await _context.Movies.FindAsync(movieId);
        if (movie != null)
        {
            movie.Rating = reviews.Any() 
                ? reviews.Average(r => r.Rating) 
                : 0;

            await _context.SaveChangesAsync();
        }
    }
}