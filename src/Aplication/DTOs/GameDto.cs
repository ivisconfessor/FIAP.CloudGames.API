using System.ComponentModel.DataAnnotations;

namespace FIAP.CloudGames.API.DTOs;

public record CreateGameDto(
    [Required] [StringLength(100)] string Title,
    [Required] string Description,
    [Required] [Range(0, double.MaxValue)] decimal Price,
    string? ImageUrl);

public record UpdateGameDto(
    [Required] [StringLength(100)] string Title,
    [Required] string Description,
    [Required] [Range(0, double.MaxValue)] decimal Price,
    string? ImageUrl);

public record GameResponseDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record PurchaseGameDto(
    [Required] Guid GameId); 