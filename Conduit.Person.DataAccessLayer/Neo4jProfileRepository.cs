using Conduit.Person.BusinessLogic;
using Conduit.Shared.Events.Models.Users.Register;
using Conduit.Shared.Events.Models.Users.Update;
using Neo4j.Driver;

namespace Conduit.Person.DataAccessLayer;

public class Neo4JProfileRepository : IProfileRepository, IAsyncDisposable
{
    private readonly Neo4JDriverFactory _driverFactory;
    private IDriver? _driver;
    private IAsyncSession? _session;

    public Neo4JProfileRepository(
        Neo4JDriverFactory driverFactory)
    {
        _driverFactory = driverFactory;
    }

    public IDriver Driver => _driver ??= _driverFactory.CreateDriver();

    public IAsyncSession Session => _session ??= Driver.AsyncSession();

    public async ValueTask DisposeAsync()
    {
        await (_session?.CloseAsync() ?? Task.CompletedTask);
        await (_driver?.CloseAsync() ?? Task.CompletedTask);
        _driver?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task SaveAfterRegisterEventAsync(
        RegisterUserEventModel registerUserEventModel)
    {
        await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(@"MERGE (p:Profile {id: $Id})
ON CREATE
SET 
    p.id = $Id,
    p.username = $Username, 
    p.email = $Email,
    p.image = $Image,
    p.biography = $Biography
RETURN p", registerUserEventModel.ToDictionary());
            if (await result.FetchAsync() == false)
            {
                throw new Neo4JApplicationException(
                    $"Failed saving during registration {registerUserEventModel.Id}");
            }
        });
    }

    public async Task SaveAfterUpdateEventAsync(
        UpdateUserEventModel updateUserEventModel)
    {
        await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(@"MERGE (p:Profile {id: $Id})
ON MATCH
SET 
    p.id = $Id,
    p.username = $Username, 
    p.email = $Email,
    p.image = $Image,
    p.biography = $Biography
RETURN p", updateUserEventModel.ToDictionary());
            if (await result.FetchAsync() == false)
            {
                throw new Neo4JApplicationException(
                    $"Failed saving during updating {updateUserEventModel.Id}");
            }
        });
    }

    public async Task<ProfileResponse?> FindAsync(
        FollowingInfo info)
    {
        return await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(
                @"MATCH (following:Profile {username: $FollowingUsername})
MATCH (follower:Profile {username: $FollowerUserId})
OPTIONAL MATCH (follower)-[relation:FOLLOW]->(following)
RETURN
following.username AS username,
following.biography AS biography,
following.image AS image,
relation IS NOT NULL AS following", info.ToDictionary());
            return await Coalesce(result);
        });
    }

    public async Task<ProfileResponse?> AddFollowingAsync(
        FollowingInfo info)
    {
        return await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(
                @"MATCH (following:Profile {username: $FollowingUsername})
MATCH (follower:Profile {id: $FollowerUserId})
MERGE MATCH (follower)-[relation:FOLLOW]->(following)
RETURN
following.username AS username,
following.biography AS biography,
following.image AS image,
true AS following", info.ToDictionary());
            return await Coalesce(result);
        });
    }

    public async Task<ProfileResponse?> RemoveFollowingAsync(
        FollowingInfo info)
    {
        return await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(
                @"MATCH (following:Profile {username: $FollowingUsername})
MATCH (follower:Profile {id: $FollowerUserId})
OPTIONAL MATCH (follower)-[relation:FOLLOW]->(following)
DELETE relation
RETURN
following.id AS id,
following.username AS username,
following.email AS email,
following.biography AS biography,
following.image AS image,
false AS following", info.ToDictionary());
            return await Coalesce(result);
        });
    }

    private static async Task<ProfileResponse?> Coalesce(
        IResultCursor result)
    {
        var records = await result.ToListAsync();
        if (records.Any() == false)
        {
            return null;
        }

        var profileRecord = records.First();
        return profileRecord.ToProfileResponse();
    }
}
