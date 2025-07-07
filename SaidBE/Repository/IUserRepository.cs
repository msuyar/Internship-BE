using LMS.Data.Entities;

namespace AspNETWebAPIDersleri.Repository;

public interface IUserRepository
{
    IQueryable<User> GetAll();
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task<bool> EmailExistsAsync(string email);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task AddMovieToWatchlistAsync(Guid userId, Guid movieId);
    Task<List<Guid>> GetWatchedMoviesAsync(Guid userId);
    Task DeleteMovieFromWatchlistAsync(Guid userId, Guid movieId);
}