namespace Conduit.Person.BusinessLogic;

public interface IProfileViewer
{
    Task<ExecutionResult<ProfileResponse>> ReturnProfile(
        FollowingInfo followingInfo,
        CancellationToken cancellationToken);
}
