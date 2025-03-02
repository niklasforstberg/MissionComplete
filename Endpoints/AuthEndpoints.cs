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

        // Public endpoints (no auth required)
        var publicGroup = group.MapGroup("")
            .WithTags("Public");
        MapPublicEndpoints(publicGroup);

        // Protected endpoints (auth required)
        var protectedGroup = group.MapGroup("")
            .RequireAuthorization()
            .WithTags("Authenticated");
        MapProtectedEndpoints(protectedGroup);

        // Admin endpoints
        var adminGroup = group.MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithTags("Admin");
        MapAdminEndpoints(adminGroup);
    }

    private static void MapPublicEndpoints(IEndpointRouteBuilder group)
    {
        // Login endpoint
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

        // First admin setup endpoint
        group.MapPost("/setup/first-admin", async (CreateAdminRequest request, ApplicationDbContext db, IConfiguration config) =>
        {
            if (await db.Users.AnyAsync(u => u.Role == User.UserRole.Admin))
            {
                return Results.BadRequest("Admin already exists. Use regular admin creation endpoint.");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = User.UserRole.Admin
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = GenerateJwtToken(user, config);
            return Results.Ok(new { Token = token });
        })
        .WithOpenApi();

        // Register endpoint
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
        .WithOpenApi();
    }

    private static void MapProtectedEndpoints(IEndpointRouteBuilder group)
    {
        // Current user endpoint
        group.MapGet("/me", async (HttpContext context, ApplicationDbContext db) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Results.Unauthorized();

            var user = await db.Users
                .Include(u => u.TeamUsers)
                    .ThenInclude(tu => tu.Team)
                .Include(u => u.InvitedBy)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null)
                return Results.NotFound();

            return Results.Ok(new
            {
                id = user.Id,
                email = user.Email,
                role = user.Role.ToString(),
                invited = user.Invited,
                invitedBy = user.InvitedBy == null ? null : new
                {
                    id = user.InvitedBy.Id,
                    email = user.InvitedBy.Email,
                    role = user.InvitedBy.Role.ToString()
                },
                teams = user.TeamUsers.Select(tu => new
                {
                    id = tu.Team.Id,
                    name = tu.Team.Name,
                    role = tu.Role.ToString(),
                    joinedAt = tu.JoinedAt
                })
            });
        })
        .WithName("GetCurrentUser")
        .WithOpenApi();
    }

    private static void MapAdminEndpoints(IEndpointRouteBuilder group)
    {
        // Admin creation endpoint
        group.MapPost("/admin/create", async (CreateAdminRequest request, ApplicationDbContext db) =>
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
        .WithOpenApi();
    }

    private static string GenerateJwtToken(User user, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException("User email is required")),
            new Claim(ClaimTypes.Role, user.Role.ToString() ?? throw new InvalidOperationException("User role is required")),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? throw new InvalidOperationException("User ID is required"))
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