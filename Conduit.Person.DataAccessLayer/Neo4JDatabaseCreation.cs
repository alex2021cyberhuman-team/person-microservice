namespace Conduit.Person.DataAccessLayer;

public class Neo4JDatabaseCreation
{
    private readonly Neo4JDriverFactory _driverFactory;

    public Neo4JDatabaseCreation(
        Neo4JDriverFactory driverFactory)
    {
        _driverFactory = driverFactory;
    }

    public async Task CreateAsync()
    {
        using var driver = _driverFactory.CreateDriver();
        await using var session = driver.AsyncSession();
        await session.WriteTransactionAsync(async tx =>
        {
            _ = await tx.RunAsync(@"CREATE CONSTRAINT 
const_profile_id_unique
IF NOT EXISTS
FOR (p:Profile)
REQUIRE p.id IS UNIQUE");
            _ = await tx.RunAsync(@"CREATE CONSTRAINT 
const_profile_username_unique 
IF NOT EXISTS
FOR (p:Profile)
REQUIRE p.username IS UNIQUE");
        });
    }
}
