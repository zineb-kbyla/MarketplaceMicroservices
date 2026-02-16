using Product.API.Application.Interfaces;
using Product.API.Application.Services;
using Product.API.Infrastructure.Data;
using Product.API.Infrastructure.Messaging;
using Product.API.Infrastructure.Repositories;
using Product.API.Infrastructure;
using Product.API.API.Middleware;
using AutoMapper;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// MongoDB Configuration
var mongoSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>();
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<DataSeeder>();

// RabbitMQ Configuration
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();
builder.Services.AddSingleton(rabbitMqSettings);
builder.Services.AddSingleton<IEventPublisher, EventPublisher>();

// Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
});

// Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

// Custom Middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Initialize MongoDB indexes and seed data
using (var scope = app.Services.CreateScope())
{
    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await mongoContext.InitializeAsync();

    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await dataSeeder.SeedAsync();
}

app.Run();

// Make the implicit Program class public for integration tests
public partial class Program { }
