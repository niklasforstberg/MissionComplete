using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MissionComplete.Data;
using MissionComplete.Models;
using MissionComplete.Models.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using MissionComplete.Models.DTOs.User;
using Microsoft.AspNetCore.Authorization;
namespace MissionComplete.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        // Public endpoints (no auth required)
        app.MapPost("/api/auth/login", async (LoginDto request, ApplicationDbContext db, IConfiguration config) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Results.Unauthorized();

            if (!user.HasPasswordSet)
                return Results.BadRequest(new { RequiresPasswordSet = true, Message = "Password needs to be set" });

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Results.Unauthorized();

            var token = GenerateJwtToken(user, config);
            return Results.Ok(new { Token = token });
        });

        app.MapPost("/api/auth/register", async (RegisterDto request, ApplicationDbContext db, IConfiguration config) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest("Email already registered");
            }

            var userRole = request.Role == TeamUser.TeamRole.Coach
                ? User.UserRole.Coach
                : User.UserRole.Player;

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = userRole
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = GenerateJwtToken(user, config);
            return Results.Ok(new { Token = token });
        });

        app.MapPost("/api/auth/setup/first-admin", async (CreateAdminDto request, ApplicationDbContext db, IConfiguration config) =>
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
        });

        // Protected endpoints (auth required)
        app.MapGet("/api/auth/me", [Authorize] async (HttpContext context, ApplicationDbContext db) =>
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

            var response = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role?.ToString() ?? "User",
                HasPasswordSet = user.HasPasswordSet,
                Invited = user.Invited,
                InvitedBy = user.InvitedBy == null ? null : new UserInviterDto
                {
                    Id = user.InvitedBy.Id,
                    Email = user.InvitedBy.Email,
                    Role = user.InvitedBy.Role?.ToString() ?? "User"
                },
                Teams = user.TeamUsers.Select(tu => new UserTeamDto
                {
                    Id = tu.Team.Id,
                    Name = tu.Team.Name,
                    Role = tu.Role.ToString(),
                    JoinedAt = tu.JoinedAt
                }).ToList()
            };

            return Results.Ok(response);
        });

        // Admin endpoints
        app.MapPost("/api/auth/admin/create", async (CreateAdminDto request, ApplicationDbContext db) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest("Email already registered");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = User.UserRole.Admin,
                HasPasswordSet = true
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok("Admin user created successfully");
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        // Inside MapAuthEndpoints method, add this before the closing brace:
#if DEBUG
        app.MapGet("/api/auth/dev-login", async (ApplicationDbContext db, IConfiguration config) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == 1);
            if (user == null) return Results.NotFound();

            // Generate token with 1 year expiration for dev login
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
                expires: DateTime.Now.AddYears(1),
                signingCredentials: credentials
            );

            return Results.Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
        });
#endif

        // Add this new endpoint
        app.MapPost("/api/auth/set-password", [Authorize] async (SetPasswordDto request, HttpContext context, ApplicationDbContext db) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Results.Unauthorized();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            if (user == null)
                return Results.NotFound();

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.HasPasswordSet = true;
            await db.SaveChangesAsync();

            return Results.Ok(new { Message = "Password set successfully" });
        });
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