using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MissionComplete.Data;
using MissionComplete.Models;
using MissionComplete.Models.DTOs;
using Microsoft.EntityFrameworkCore;
namespace MissionComplete.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", async (LoginRequest request, ApplicationDbContext db, IConfiguration config) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            var token = GenerateJwtToken(user, config);
            return Results.Ok(new { Token = token });
        });

        group.MapPost("/dev/create-admin", async (CreateAdminRequest request, ApplicationDbContext db) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest("Email already registered");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = User.UserRole.Admin
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok("Admin user created successfully");
        })
        .WithTags("Development")
        .WithOpenApi();

        group.MapPost("/register", async (RegisterRequest request, ApplicationDbContext db, IConfiguration config) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest("Email already registered");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.IsCoach ? User.UserRole.Coach : User.UserRole.Player
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = GenerateJwtToken(user, config);
            return Results.Ok(new { Token = token });
        })
        .WithTags("Authentication")
        .WithOpenApi();
    }

    private static string GenerateJwtToken(User user, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 