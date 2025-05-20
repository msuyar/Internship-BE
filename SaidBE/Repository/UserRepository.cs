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
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
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
    
    public async Task UpdateAsync(User user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
    
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID {user.Id} not found.");
        }
        
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.UpdatedAt = DateTime.UtcNow;

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}