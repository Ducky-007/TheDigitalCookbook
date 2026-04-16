using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDigitalCookbook.Data;
using TheDigitalCookbook.DTOs;
using TheDigitalCookbook.Models;
using TheDigitalCookbook.Security;

namespace TheDigitalCookbook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new LoginResponse
            {
                Success = false,
                Message = "Username and password are required."
            });
        }

        var existingUser = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (existingUser)
        {
            return Conflict(new LoginResponse
            {
                Success = false,
                Message = "Username is already taken."
            });
        }

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new LoginResponse
        {
            Success = true,
            Message = "Registration successful."
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            return Unauthorized(new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password."
            });
        }

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password."
            });
        }

        return Ok(new LoginResponse
        {
            Success = true,
            Message = "Login successful."
        });
    }
}
