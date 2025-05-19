using LMS.Data.Entities;

namespace AspNETWebAPIDersleri.Repository;

public interface IUserRepository
{
    IQueryable<User> GetAll();
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> DeleteAsync(Guid id);
}