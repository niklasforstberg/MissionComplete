using MissionComplete.Data;
using Microsoft.EntityFrameworkCore;
using MissionComplete.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using MissionComplete.Integrations;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON options to handle enums as strings and use PascalCase
 // This ensures all API responses use PascalCase property names
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = null; // null = PascalCase (no conversion)
    options.SerializerOptions.WriteIndented = false;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger to use PascalCase in schema generation
    options.UseInlineDefinitionsForEnums();
    options.SupportNonNullableReferenceTypes();
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below. *Bearer is not needed, just the token*"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")))
        };
    });

builder.Services.AddAuthorization();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback to environment variable for Docker support
        connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
    }
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string not found"));
});

builder.Services.AddScoped<SmtpEmailSender>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Register API endpoints
app.MapAuthEndpoints();
app.MapTeamEndpoints();
app.MapUserEndpoints();
app.MapChallengeEndpoints();
app.MapGoalEndpoints();

// Serve static files from clientapp/dist
var clientAppPath = Path.Combine(builder.Environment.ContentRootPath, "clientapp", "dist");
if (Directory.Exists(clientAppPath))
{
    var fileProvider = new PhysicalFileProvider(clientAppPath);

    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = fileProvider
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = fileProvider,
        RequestPath = ""
    });

    // Serve client app for all non-API routes
    app.MapFallback(async (HttpContext context) =>
    {
        // Don't serve SPA for API routes
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 404;
            return;
        }

        context.Response.ContentType = "text/html";
        var indexFile = fileProvider.GetFileInfo("index.html");
        if (indexFile.Exists)
        {
            await context.Response.SendFileAsync(indexFile);
        }
        else
        {
            context.Response.StatusCode = 404;
        }
    });
}

app.Run();

