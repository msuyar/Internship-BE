using LMS.Data.Entities;

public interface IMovieRepository
{
    IQueryable<Movie> GetAll();
    Task<(bool found, Movie? movie)> GetByIdAsync(int id);
    Task AddAsync(Movie movie);
    Task UpdateAsync(Movie movie);
    Task DeleteAsync(int id);
}