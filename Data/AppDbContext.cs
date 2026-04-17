using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TheDigitalCookbook.Models;

namespace TheDigitalCookbook.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var stringListConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.Property(r => r.Ingredients)
                .HasConversion(stringListConverter)
                .HasColumnType("json");

            entity.Property(r => r.Instructions)
                .HasConversion(stringListConverter)
                .HasColumnType("json");
        });

        base.OnModelCreating(modelBuilder);
    }
}