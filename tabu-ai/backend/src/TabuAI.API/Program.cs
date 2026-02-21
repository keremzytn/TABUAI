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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
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

// Database Migration (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TabuAIDbContext>();
    try
    {
        await context.Database.MigrateAsync();
        app.Logger.LogInformation("Database migrations applied");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while ensuring the database was created");
    }
}

app.Logger.LogInformation("TABU.AI API is starting up...");

app.Run();
