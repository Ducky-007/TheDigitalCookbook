using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDigitalCookbook.Data;
using TheDigitalCookbook.DTOs;
using TheDigitalCookbook.Models;
using TheDigitalCookbook.Security;

namespace TheDigitalCookbook.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        var existingUser = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (existingUser)
        {
            TempData["UsernameTaken"] = "Username is already taken.";
            return View();
        }

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        TempData["UserAdded"] = $"{newUser.Username} added successfully!";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            TempData["LoginError"] = "Invalid username or password.";
            return View();
        }
        
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.Username);
        
        return RedirectToAction("Index", "Recipe");
    }
}