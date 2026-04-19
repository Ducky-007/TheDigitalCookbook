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

    [HttpGet]
    public async Task<IActionResult> Report(string? query)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var username = HttpContext.Session.GetString("Username");

        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var recipesQuery = _db.Recipes
            .Where(r => r.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(query))
        {
            recipesQuery = recipesQuery.Where(r => r.Name.Contains(query));
        }

        var recipes = await recipesQuery
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

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

    public IActionResult Details(int id)
    {
        var recipe = _db.Recipes.FirstOrDefault(r => r.Id == id);

        if (recipe == null)
        {
            TempData["RecipeNotFound"] = "Recipe not found." ;
            return RedirectToAction("Index", "Recipe");
        }
        return View(recipe);
    }
    
    [HttpPost]
    public async Task<IActionResult> Details(int id, string name, string category, string ingredients, string instructions, string prepTime, string cookTime, string? url)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId.Value);
        if (recipe == null)
        {
            return NotFound();
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
            return View(recipe);
        }

        recipe.Name = name.Trim();
        recipe.Category = category.Trim();
        recipe.Ingredients = ingredients
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();
        recipe.Instructions = instructions
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();
        recipe.PrepTime = parsedPrepTime;
        recipe.CookTime = parsedCookTime;
        recipe.URL = string.IsNullOrWhiteSpace(url) ? null : url.Trim();

        await _db.SaveChangesAsync();
        TempData["RecipeAdded"] = $"{recipe.Name} updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Auth");

        var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId.Value);
    
        if (recipe != null)
        {
            _db.Recipes.Remove(recipe);
            await _db.SaveChangesAsync();
            TempData["RecipeDeleted"] = "Deleted successfully!";
        }

        return RedirectToAction("Index");
    }
}