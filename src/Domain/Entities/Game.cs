using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.API.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; }
    
    [Required]
    [StringLength(100)]
    public string Title { get; private set; }
    
    [Required]
    public string Description { get; private set; }
    
    [Required]
    public decimal Price { get; private set; }
    
    public string? ImageUrl { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Game() { } // Para o EF Core

    public Game(string title, string description, decimal price, string? imageUrl = null)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string description, decimal price, string? imageUrl = null)
    {
        Title = title;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
} 