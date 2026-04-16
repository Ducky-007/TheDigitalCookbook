using System;
using System.Collections.Generic;
using System.Linq;

namespace TheDigitalCookbook;

public class Recipe
{
    // recipe details
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Name { get; private set; }
    public string Category { get; private set; }
    public List<string> Ingredients { get; private set; }
    public List<string> Instructions { get; private set; }
    public TimeSpan PrepTime { get; private set; }
    public TimeSpan CookTime { get; private set; }
    public TimeSpan TotalTime => PrepTime + CookTime;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string URL { get; private set; }

    public Recipe(
        string name,
        string category,
        List<string> ingredients,
        List<string> instructions,
        TimeSpan prepTime,
        TimeSpan cookTime,
        string url)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty.", nameof(category));

        if (ingredients == null || ingredients.Count == 0)
            throw new ArgumentException("Ingredients cannot be empty.", nameof(ingredients));

        if (ingredients.Any(i => string.IsNullOrWhiteSpace(i)))
            throw new ArgumentException("Ingredients cannot contain empty items.", nameof(ingredients));

        if (instructions == null || instructions.Count == 0)
            throw new ArgumentException("Instructions cannot be empty.", nameof(instructions));

        if (instructions.Any(i => string.IsNullOrWhiteSpace(i)))
            throw new ArgumentException("Instructions cannot contain empty items.", nameof(instructions));

        if (prepTime < TimeSpan.Zero)
            throw new ArgumentException("PrepTime cannot be negative.", nameof(prepTime));

        if (cookTime < TimeSpan.Zero)
            throw new ArgumentException("CookTime cannot be negative.", nameof(cookTime));

        if (!string.IsNullOrWhiteSpace(url) && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new ArgumentException("URL must be a valid absolute URL.", nameof(url));

        // Assign validated values
        Name = name.Trim();
        Category = category.Trim();
        Ingredients = ingredients.Select(i => i.Trim()).ToList();
        Instructions = instructions.Select(i => i.Trim()).ToList();
        PrepTime = prepTime;
        CookTime = cookTime;
        URL = url?.Trim() ?? string.Empty;
    }
}