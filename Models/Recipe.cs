using System;
using System.Collections.Generic;
using System.Linq;

namespace TheDigitalCookbook;

public class Recipe
{
    // EF Core needs a parameterless constructor
    public Recipe()
    {
        Ingredients = new List<string>();
        Instructions = new List<string>();
    }

    public int Id { get; set; }
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();

    public TimeSpan PrepTime { get; set; }
    public TimeSpan CookTime { get; set; }

    public TimeSpan TotalTime => PrepTime + CookTime;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string URL { get; set; } = string.Empty;

    public Recipe(
        string name,
        string category,
        List<string> ingredients,
        List<string> instructions,
        TimeSpan prepTime,
        TimeSpan cookTime,
        string url)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty.", nameof(category));

        if (ingredients == null || ingredients.Count == 0)
            throw new ArgumentException("Ingredients cannot be empty.", nameof(ingredients));

        if (instructions == null || instructions.Count == 0)
            throw new ArgumentException("Instructions cannot be empty.", nameof(instructions));

        if (prepTime < TimeSpan.Zero)
            throw new ArgumentException("PrepTime cannot be negative.", nameof(prepTime));

        if (cookTime < TimeSpan.Zero)
            throw new ArgumentException("CookTime cannot be negative.", nameof(cookTime));

        if (!string.IsNullOrWhiteSpace(url) && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new ArgumentException("URL must be a valid absolute URL.", nameof(url));

        Name = name.Trim();
        Category = category.Trim();
        Ingredients = ingredients.Select(i => i.Trim()).ToList();
        Instructions = instructions.Select(i => i.Trim()).ToList();
        PrepTime = prepTime;
        CookTime = cookTime;
        URL = url?.Trim() ?? string.Empty;
    }
}