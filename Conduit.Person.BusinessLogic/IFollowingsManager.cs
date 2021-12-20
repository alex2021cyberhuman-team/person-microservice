namespace Conduit.Person.BusinessLogic;

public interface IFollowingsManager
{
    Task<ExecutionResult<ProfileResponse>> AssignFollowingAsync(
        FollowingInfo followingInfo,
        CancellationToken cancellationToken = default);

    Task<ExecutionResult<ProfileResponse>> DeAssignFollowingAsync(
        FollowingInfo followingInfo,
        CancellationToken cancellationToken = default);
}
