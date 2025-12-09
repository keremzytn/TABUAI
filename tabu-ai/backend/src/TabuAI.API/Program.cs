using Microsoft.EntityFrameworkCore;
using TabuAI.Application.Common.Mappings;
using TabuAI.Application.Features.Game.Commands;
using TabuAI.Domain.Interfaces;
using TabuAI.Infrastructure.Data;
using TabuAI.Infrastructure.Repositories;
using TabuAI.Infrastructure.Services;

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

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(StartGameCommand).Assembly));

// AI Service (Groq)
builder.Services.AddScoped<IAiService, GroqService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
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

app.UseHttpsRedirection();
app.UseCors("AllowAngularDev");

app.UseAuthorization();

app.MapControllers();

// Database Migration (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TabuAIDbContext>();
    try
    {
        await context.Database.EnsureCreatedAsync();
        app.Logger.LogInformation("Database ensured created");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while ensuring the database was created");
    }
}

app.Logger.LogInformation("TABU.AI API is starting up...");

app.Run();
