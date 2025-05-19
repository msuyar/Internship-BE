using LMS.Data;
using LMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNETWebAPIDersleri.Repository;

public class UserRepository : IUserRepository
{
    private readonly LMSDBContext _context;

    public UserRepository(LMSDBContext context)
    {
        _context = context;
    }
    
    public IQueryable<User> GetAll()
    {
        return _context.Users.AsQueryable();
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
        {
            return false;
        }
            

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}