namespace Conduit.Person.BusinessLogic;

public record FollowingInfo(
    string FollowedUsername,
    Guid FollowerUserId);
