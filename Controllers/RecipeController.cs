using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDigitalCookbook.Data;
using TheDigitalCookbook;

namespace TheDigitalCookbook.Controllers;

public class RecipeController : Controller
{
    private readonly AppDbContext _db;
    
    public RecipeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var username = HttpContext.Session.GetString("Username");

        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var recipes = await _db.Recipes
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        ViewBag.Username = username ?? "User";
        return View(recipes);
    }

    public IActionResult Create()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string name, string category, string ingredients, string instructions, string prepTime, string cookTime, string? url)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!TimeSpan.TryParse(prepTime, out var parsedPrepTime))
        {
            ModelState.AddModelError(nameof(prepTime), "Invalid prep time.");
        }

        if (!TimeSpan.TryParse(cookTime, out var parsedCookTime))
        {
            ModelState.AddModelError(nameof(cookTime), "Invalid cook time.");
        }

        if (!ModelState.IsValid)
        {
            //write console line for testing
            foreach (var modelState in ModelState.Values) {
                foreach (var error in modelState.Errors) {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View();
        }

        var recipe = new Recipe
        {
            UserId = userId.Value,
            Name = name.Trim(),
            Category = category.Trim(),
            Ingredients = ingredients
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList(),
            Instructions = instructions
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList(),
            PrepTime = parsedPrepTime,
            CookTime = parsedCookTime,
            URL = string.IsNullOrWhiteSpace(url) ? null : url.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();
        TempData["RecipeAdded"] = $"{recipe.Name} added successfully!";

        return RedirectToAction("Index", "Recipe");
    }
}