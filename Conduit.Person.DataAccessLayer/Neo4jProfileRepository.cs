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
        var dictionary = registerUserEventModel.ToDictionary();
        await MergeProfileAsync(dictionary);
    }

    public async Task SaveAfterUpdateEventAsync(
        UpdateUserEventModel updateUserEventModel)
    {
        var dictionary = updateUserEventModel.ToDictionary();
        await MergeProfileAsync(dictionary);
    }

    public async Task<(ProfileResponse?, Guid?)> FindAsync(
        FollowingInfo info)
    {
        return await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(
                @"MATCH (followed:Profile {username: $FollowedUsername})
OPTIONAL MATCH (follower:Profile {id: $FollowerUserId})
OPTIONAL MATCH (follower)-[relation:FOLLOW]->(followed)
RETURN
followed.id AS id,
followed.username AS username,
followed.email AS email,
followed.biography AS biography,
followed.image AS image,
relation IS NOT NULL AS followed", info.ToDictionary());
            return await Coalesce(result);
        });
    }

    public async Task<(ProfileResponse?, Guid?)> AddFollowingAsync(
        FollowingInfo info)
    {
        return await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(
                @"MATCH (followed:Profile {username: $FollowedUsername})
MATCH (follower:Profile {id: $FollowerUserId})
MERGE (follower)-[relation:FOLLOW]->(followed)
RETURN
followed.id AS id,
followed.username AS username,
followed.email AS email,
followed.biography AS biography,
followed.image AS image,
true AS followed", info.ToDictionary());
            return await Coalesce(result);
        });
    }

    public async Task<(ProfileResponse?, Guid?)> RemoveFollowingAsync(
        FollowingInfo info)
    {
        return await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(
                @"MATCH (followed:Profile {username: $FollowedUsername})
MATCH (follower:Profile {id: $FollowerUserId})
OPTIONAL MATCH (follower)-[relation:FOLLOW]->(followed)
DELETE relation
RETURN
followed.id AS id,
followed.username AS username,
followed.email AS email,
followed.biography AS biography,
followed.image AS image,
false AS followed", info.ToDictionary());
            return await Coalesce(result);
        });
    }

    private async Task MergeProfileAsync(
        IDictionary<string, object?> dictionary)
    {
        await Session.WriteTransactionAsync(async tx =>
        {
            var result = await tx.RunAsync(@"MERGE (p:Profile {id: $Id})
SET 
    p.username = $Username, 
    p.email = $Email,
    p.image = $Image,
    p.biography = $Biography
RETURN p", dictionary);
            if (await result.FetchAsync() == false)
            {
                throw new Neo4JApplicationException(
                    $"Failed saving during registration {dictionary["Id"]}");
            }
        });
    }

    private static async Task<(ProfileResponse?, Guid?)> Coalesce(
        IResultCursor result)
    {
        var records = await result.ToListAsync();
        if (records.Any() == false)
        {
            return (null, null);
        }

        var profileRecord = records.First();
        return profileRecord.ToProfileResponse();
    }
}
