using MassTransit;
using Recommendation.API.Infrastructure.Data;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Infrastructure.Repositories;
using Recommendation.API.Application.Services;
using Recommendation.API.Application.Algorithms;
using Recommendation.API.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Configure services
var neo4jSettings = builder.Configuration.GetSection("Neo4j").Get<Neo4jSettings>() 
    ?? throw new InvalidOperationException("Neo4j settings not configured");

builder.Services.AddSingleton(neo4jSettings);
builder.Services.AddSingleton<Neo4jContext>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<CollaborativeFilteringAlgorithm>();
builder.Services.AddScoped<ContentBasedFilteringAlgorithm>();

// Add logging
builder.Services.AddLogging();

// Add controllers
builder.Services.AddControllers();

// Add MassTransit for RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<ProductViewedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        var rabbitMqPort = builder.Configuration.GetValue<int>("RabbitMQ:Port");
        var rabbitMqUsername = builder.Configuration.GetValue<string>("RabbitMQ:Username") ?? "guest";
        var rabbitMqPassword = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? "guest";

        cfg.Host($"rabbitmq://{rabbitMqUsername}:{rabbitMqPassword}@{rabbitMqHost}:{rabbitMqPort}");

        cfg.ReceiveEndpoint("recommendation-service", e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
            e.ConfigureConsumer<ProductViewedEventConsumer>(context);
        });
    });
});

// Add Swagger
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
