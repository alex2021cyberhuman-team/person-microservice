using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Conduit.Person.DataAccessLayer;

public class Neo4JHealthCheck : IHealthCheck
{
    private readonly Neo4JDriverFactory _driverFactory;

    public Neo4JHealthCheck(
        Neo4JDriverFactory driverFactory)
    {
        _driverFactory = driverFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        try
        {
            using var driver = _driverFactory.CreateDriver();
            await using var session = driver.AsyncSession();
            await session.WriteTransactionAsync(async tx =>
            {
                _ = await tx.RunAsync(@"MATCH (n)
RETURN n.name
ORDER BY n.name
LIMIT 1");
            });
        }
        catch (Exception e)
        {
            return new(HealthStatus.Unhealthy, e.Message, e);
        }

        return new(HealthStatus.Healthy);
    }
}
