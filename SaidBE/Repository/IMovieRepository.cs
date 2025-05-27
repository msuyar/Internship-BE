using LMS.Data.Entities;

namespace AspNETWebAPIDersleri.Repository;

public interface IMovieRepository
{
    IQueryable<Movie> GetAll();
    Task <Movie?> GetByIdAsync(Guid id);
    Task AddAsync(Movie movie);
    Task UpdateAsync(Movie movie);
    Task DeleteAsync(Guid id);
}