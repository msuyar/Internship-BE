using AspNETWebAPIDersleri.Repository;
using LMS.Data.Dtos;
using LMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNETWebAPIDersleri.Utils;

namespace AspNETWebAPIDersleri.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _repository;

    public AuthController(IUserRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _repository.GetAll().ToListAsync();

        return Ok(new
        {
            success = true,
            message = $"Retrieved {users.Count()} user(s) successfully",
            data = users
        });
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        if (await _repository.EmailExistsAsync(dto.Email))
        {
            return BadRequest(new { message = "Email already registered." });
        }
        
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = PasswordHasher.HashPassword(dto.Password)
        };

        await _repository.AddAsync(user);

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var user = await _repository.GetByEmailAsync(dto.Email);

        if (user == null || PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }
        
        return Ok(new { message = "Login successful." });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        var existingUser = await _repository.GetByIdAsync(id);

        if (existingUser == null)
        {
            return NotFound(new
            {
                success = false,
                message = $"User with ID {id} was not found."
            });
        }

        existingUser.FirstName = dto.FirstName;
        existingUser.LastName = dto.LastName;
        existingUser.Email = dto.Email;
        existingUser.UpdatedAt = DateTime.UtcNow;

        return Ok(new
        {
            success = true,
            message = "User updated successfully.",
            data = existingUser
        });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repository.DeleteAsync(id);

        return Ok(new { message = "User deleted successfully." });
    }

    [HttpPost("add-to-watchlist")]
    public async Task<IActionResult> AddToWatchlist([FromBody] AddMovieToWatchlistDto dto)
    {
        try
        {
            await _repository.AddMovieToWatchlistAsync(dto.UserId, dto.MovieId);

            return Ok(new { message = "Movie added to watchlist." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpGet("watchlist")]
    public async Task<IActionResult> GetWatchlist(Guid userId)
    {
        try
        {
            var movies = await _repository.GetWatchedMoviesAsync(userId);
            return Ok(new { watchedMovies = movies });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpGet("deletefromwatchlist")]
    public async Task<IActionResult> DeleteFromWatchlist([FromBody] AddMovieToWatchlistDto dto)
    {
        try
        {
            await _repository.DeleteMovieFromWatchlistAsync(dto.UserId, dto.MovieId);

            return Ok(new { message = "Movie removed from watchlist." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}