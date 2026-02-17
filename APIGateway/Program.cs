using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Health Checks
builder.Services.AddHealthChecks();

// Add request timeout support
builder.Services.AddRequestTimeouts();

// Add YARP ReverseProxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// Add CORS
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
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Add logging middleware
app.UseMiddleware<GatewayLoggingMiddleware>();

app.UseRouting();

// Add request timeouts BEFORE executing endpoints
app.UseRequestTimeouts();

app.UseAuthorization();

app.MapControllers();

// Map health check
app.MapHealthChecks("/health");

// Map YARP routes
app.MapReverseProxy();

app.Run();

// Custom logging middleware
public class GatewayLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayLoggingMiddleware> _logger;

    public GatewayLoggingMiddleware(RequestDelegate next, ILogger<GatewayLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Gateway: {context.Request.Method} {context.Request.Path}");
        await _next(context);
        _logger.LogInformation($"Gateway Response: {context.Response.StatusCode}");
    }
}

public partial class Program { }
