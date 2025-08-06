using System.Security.Claims;
using FIAP.CloudGames.API.Domain.Entities;
using FIAP.CloudGames.API.DTOs;
using FIAP.CloudGames.API.Infrastructure.Configurations;
using FIAP.CloudGames.API.Infrastructure.Data;
using FIAP.CloudGames.API.Infrastructure.Services;
using FIAP.CloudGames.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using FluentValidation;
using FIAP.CloudGames.API.Aplication.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FIAP Cloud Games API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Registrar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

// Integrar todas as dependências
builder.Services.IntegrateDependencyResolver(builder.Configuration);

// Configurar Application Insights
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Seed de usuário admin para desenvolvimento
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!db.Users.Any(u => u.Role == UserRole.Admin))
    {
        var admin = new User("Admin", "admin@fiap.com.br", "Admin@123", UserRole.Admin);
        db.Users.Add(admin);
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// User endpoints
app.MapPost("/api/users", async (CreateUserDto dto, ApplicationDbContext db) =>
{
    if (await db.Users.AnyAsync(u => u.Email == dto.Email))
        return Results.BadRequest("Email já cadastrado");

    var user = new User(dto.Name, dto.Email, dto.Password);
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Id}", new UserResponseDto(
        user.Id, user.Name, user.Email, user.Role.ToString(),
        user.CreatedAt, user.UpdatedAt));
})
.AllowAnonymous()
.WithName("CreateUser")
.WithOpenApi();

app.MapPost("/api/auth/login", async (LoginDto dto, ApplicationDbContext db, IJwtService jwtService) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
    if (user == null || !user.ValidatePassword(dto.Password))
        return Results.Unauthorized();

    var token = jwtService.GenerateToken(user);
    return Results.Ok(new LoginResponseDto(token, new UserResponseDto(
        user.Id, user.Name, user.Email, user.Role.ToString(),
        user.CreatedAt, user.UpdatedAt)));
})
.AllowAnonymous()
.WithName("Login")
.WithOpenApi();

// Game endpoints
app.MapPost("/api/games", async (CreateGameDto dto, ApplicationDbContext db, ClaimsPrincipal user) =>
{
    if (user.FindFirst(ClaimTypes.Role)?.Value != UserRole.Admin.ToString())
        return Results.Forbid();

    var game = new Game(dto.Title, dto.Description, dto.Price, dto.ImageUrl);
    db.Games.Add(game);
    await db.SaveChangesAsync();

    return Results.Created($"/api/games/{game.Id}", new GameResponseDto(
        game.Id, game.Title, game.Description, game.Price,
        game.ImageUrl, game.CreatedAt, game.UpdatedAt));
})
.WithName("CreateGame")
.RequireAuthorization()
.WithOpenApi();

app.MapGet("/api/games", async (ApplicationDbContext db) =>
{
    var games = await db.Games
        .Select(g => new GameResponseDto(
            g.Id, g.Title, g.Description, g.Price,
            g.ImageUrl, g.CreatedAt, g.UpdatedAt))
        .ToListAsync();

    return Results.Ok(games);
})
.WithName("GetGames")
.RequireAuthorization()
.WithOpenApi();

// User Games endpoints
app.MapPost("/api/users/games", async (PurchaseGameDto dto, ApplicationDbContext db, ClaimsPrincipal user) =>
{
    var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    var game = await db.Games.FindAsync(dto.GameId);
    
    if (game == null)
        return Results.NotFound("Jogo não encontrado");

    if (await db.UserGames.AnyAsync(ug => ug.UserId == userId && ug.GameId == dto.GameId))
        return Results.BadRequest("Jogo já adquirido");

    var userGame = new UserGame(
        await db.Users.FindAsync(userId),
        game);
    
    db.UserGames.Add(userGame);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/games/{userGame.Id}", new GameResponseDto(
        game.Id, game.Title, game.Description, game.Price,
        game.ImageUrl, game.CreatedAt, game.UpdatedAt));
})
.WithName("PurchaseGame")
.RequireAuthorization()
.WithOpenApi();

app.MapGet("/api/users/games", async (ApplicationDbContext db, ClaimsPrincipal user) =>
{
    var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    var games = await db.UserGames
        .Where(ug => ug.UserId == userId)
        .Select(ug => new GameResponseDto(
            ug.Game.Id, ug.Game.Title, ug.Game.Description, ug.Game.Price,
            ug.Game.ImageUrl, ug.Game.CreatedAt, ug.Game.UpdatedAt))
        .ToListAsync();

    return Results.Ok(games);
})
.WithName("GetUserGames")
.RequireAuthorization()
.WithOpenApi();

app.Run();
