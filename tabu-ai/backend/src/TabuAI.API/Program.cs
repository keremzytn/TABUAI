using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TabuAI.Application.Common.Behaviors;
using TabuAI.Application.Common.Mappings;
using TabuAI.Application.Common.Validators;
using TabuAI.Application.Features.Game.Commands;
using TabuAI.Domain.Interfaces;
using TabuAI.Infrastructure.Data;
using TabuAI.Infrastructure.Repositories;
using TabuAI.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TabuAI.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "TABU.AI API", 
        Version = "v1",
        Description = "TABU.AI - Prompt Engineering Oyunu API"
    });
});

// Database Configuration
builder.Services.AddDbContext<TabuAIDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// MediatR + Validation Pipeline
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(StartGameCommand).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// FluentValidation - tüm validator'ları otomatik kaydet
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

// AI Service (Groq)
builder.Services.AddScoped<IAiService, GroqService>();

// Badge Service
builder.Services.AddScoped<TabuAI.Application.Common.Services.IBadgeService, TabuAI.Application.Common.Services.BadgeService>();

// Token Service
builder.Services.AddScoped<TabuAI.Application.Common.Interfaces.ITokenService, TabuAI.Infrastructure.Services.TokenService>();

// External Auth Service
builder.Services.AddHttpClient();
builder.Services.AddScoped<TabuAI.Application.Common.Interfaces.IExternalAuthService, TabuAI.Infrastructure.Services.ExternalAuthService>();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "TabuAI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "TabuAIUsers",
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "super_secret_key_tabuia_secure_2024!")),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };

    // SignalR sends JWT via query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/game"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
              {
                  var uri = new Uri(origin);
                  return uri.Host == "localhost" || 
                         uri.Host == "127.0.0.1" || 
                         uri.Host == "172.16.0.159" ||
                         uri.Host == "carmedlaw.com" ||
                         uri.Host == "www.carmedlaw.com" ||
                         origin.StartsWith("capacitor://") ||
                         origin.StartsWith("ionic://") ||
                         uri.Host.StartsWith("192.168.") ||
                         uri.Host.StartsWith("172.16.");
              })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TABU.AI API v1");
        c.RoutePrefix = string.Empty; // Swagger UI root'ta açılır
    });
}

// app.UseHttpsRedirection();
app.UseCors("AllowAngularDev");

// Global Validation Exception Handler
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (FluentValidation.ValidationException ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
        await context.Response.WriteAsJsonAsync(new { message = "Doğrulama hatası", errors });
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/hubs/game");

// Database Migration (Automatic for all environments)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TabuAIDbContext>();
    try
    {
        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
        if (pendingMigrations.Any())
        {
            var appliedMigrations = (await context.Database.GetAppliedMigrationsAsync()).ToList();
            if (!appliedMigrations.Any())
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'Users'";
                var result = await cmd.ExecuteScalarAsync();
                var tableExists = Convert.ToInt64(result) > 0;

                if (tableExists)
                {
                    app.Logger.LogWarning("Database tables already exist but no migrations recorded. Marking Initial migration as applied...");
                    await context.Database.ExecuteSqlRawAsync(
                        "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\"MigrationId\" varchar(150) NOT NULL, \"ProductVersion\" varchar(32) NOT NULL, CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY (\"MigrationId\"))");
                    await context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20260308200605_Initial', '9.0.2') ON CONFLICT DO NOTHING");
                    app.Logger.LogInformation("Initial migration marked as applied.");

                    pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
                }
            }

            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
                app.Logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                app.Logger.LogInformation("Database is up to date, no pending migrations.");
            }
        }
        else
        {
            app.Logger.LogInformation("Database is up to date, no pending migrations.");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred during database migration");
    }
}

app.Logger.LogInformation("TABU.AI API is starting up...");

app.Run();
