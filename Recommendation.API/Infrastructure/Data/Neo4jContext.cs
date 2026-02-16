using Neo4j.Driver;

namespace Recommendation.API.Infrastructure.Data
{
    public class Neo4jSettings
    {
        public string Uri { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Database { get; set; } = "neo4j";
        public int ConnectionTimeout { get; set; } = 30;
    }

    public class Neo4jContext
    {
        private readonly IDriver _driver;
        private readonly string _database;
        private readonly ILogger<Neo4jContext> _logger;

        public Neo4jContext(Neo4jSettings settings, ILogger<Neo4jContext> logger)
        {
            _logger = logger;
            _database = settings.Database;

            try
            {
                _driver = GraphDatabase.Driver(
                    settings.Uri,
                    AuthTokens.Basic(settings.Username, settings.Password)
                );

                _logger.LogInformation("Neo4j driver initialized for database {Database}", _database);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Neo4j driver");
                throw;
            }
        }

        public async Task<IAsyncSession> GetSessionAsync()
        {
            return _driver.AsyncSession(options => options.WithDatabase(_database));
        }

        public async Task<IResultCursor> ExecuteQueryAsync(string query, IDictionary<string, object>? parameters = null)
        {
            var session = await GetSessionAsync();
            try
            {
                _logger.LogDebug("Executing Cypher query: {Query}", query);
                return await session.RunAsync(query, parameters ?? new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query: {Query}", query);
                await session.CloseAsync();
                throw;
            }
        }

        public async Task CloseAsync()
        {
            await _driver.DisposeAsync();
            _logger.LogInformation("Neo4j driver closed");
        }
    }
}
