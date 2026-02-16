using Order.API.Application.Interfaces;
using Order.API.Application.Services;
using Order.API.Infrastructure.Data;
using Order.API.Infrastructure.Messaging;
using Order.API.Infrastructure.Repositories;
using Order.API.Infrastructure;
using Order.API.API.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
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
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register Services
builder.Services.AddScoped<IOrderService, OrderService>();

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

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

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
