using Conduit.Person.BusinessLogic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Conduit.Person.DataAccessLayer;

public static class Neo4JRegistrationExtensions
{
    public static IServiceCollection RegisterNeo4JWithHealthCheck(
        this IServiceCollection services,
        Action<Neo4JOptions> configureOptions)
    {
        return services.AddHealthChecks()
            .AddCheck<Neo4JHealthCheck>("neo4j", HealthStatus.Unhealthy)
            .Services.Configure(configureOptions)
            .AddScoped<IProfileRepository, Neo4JProfileRepository>()
            .AddSingleton<Neo4JDriverFactory>()
            .AddScoped<Neo4JDatabaseCreation>();
    }

    public static async Task<IServiceScope> InitializeNeo4JAsync(
        this IServiceScope scope)
    {
        var databaseCreation = scope.ServiceProvider
            .GetRequiredService<Neo4JDatabaseCreation>();
        await databaseCreation.CreateAsync();
        return scope;
    }
}
