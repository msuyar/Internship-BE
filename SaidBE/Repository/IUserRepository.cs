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
}