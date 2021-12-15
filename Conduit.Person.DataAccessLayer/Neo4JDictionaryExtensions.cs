using Conduit.Person.BusinessLogic;
using Conduit.Shared.Events.Models.Users.Register;
using Conduit.Shared.Events.Models.Users.Update;
using Neo4j.Driver;

namespace Conduit.Person.DataAccessLayer;

public static class Neo4JDictionaryExtensions
{
    public static IDictionary<string, object?> ToDictionary(
        this RegisterUserEventModel model) =>
        new Dictionary<string, object?>()
        {
            ["Id"] = model.Id.ToString(),
            ["Username"] = model.Username,
            ["Email"] = model.Email,
            ["Image"] = model.Image,
            ["Biography"] = model.Biography,
        };
    
    public static IDictionary<string, object?> ToDictionary(
        this UpdateUserEventModel model) =>
        new Dictionary<string, object?>()
        {
            ["Id"] = model.Id.ToString(),
            ["Username"] = model.Username,
            ["Email"] = model.Email,
            ["Image"] = model.Image,
            ["Biography"] = model.Biography,
        };
    
    public static IDictionary<string, object?> ToDictionary(
        this FollowingInfo model) =>
        new Dictionary<string, object?>()
        {
            ["FollowerUserId"] = model.FollowerUserId,
            ["FollowingUsername"] = model.FollowingUsername
        };
    
    public static ProfileResponse ToProfileResponse(this IRecord profileRecord) => new(
        profileRecord["username"].As<string>(),
        profileRecord["image"].As<string>(),
        profileRecord["biography"].As<string>(),
        profileRecord["following"].As<bool>());
}
