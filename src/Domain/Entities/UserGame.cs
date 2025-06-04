namespace FIAP.CloudGames.API.Domain.Entities;

public class UserGame
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public decimal PurchasePrice { get; private set; }

    public User User { get; private set; }
    public Game Game { get; private set; }

    private UserGame() { } // Para o EF Core

    public UserGame(User user, Game game)
    {
        Id = Guid.NewGuid();
        UserId = user.Id;
        GameId = game.Id;
        PurchaseDate = DateTime.UtcNow;
        PurchasePrice = game.Price;
        User = user;
        Game = game;
    }
} 