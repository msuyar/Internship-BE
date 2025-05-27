using LMS.Data.Entities;

namespace AspNETWebAPIDersleri.Repository;

public interface IReviewRepository
{ 
        Task<bool> HasUserReviewedMovieAsync(Guid userId, Guid movieId); 
        Task AddReviewAsync(Review review);
        Task<bool> UserExistsAsync(Guid userId);
        Task<bool> MovieExistsAsync(Guid movieId);
        Task<Review?> GetUserReviewAsync(Guid userId, Guid movieId);
        Task UpdateReviewAsync(Review review);
        Task<List<Review>> GetAllReviewsAsync();
        Task UpdateMovieAverageRating(Guid movieId);
}
