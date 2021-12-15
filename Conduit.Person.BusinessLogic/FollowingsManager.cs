using System.Net;

namespace Conduit.Person.BusinessLogic;

public class FollowingsManager : IFollowingsManager
{
    private readonly IProfileRepository _profileRepository;

    public FollowingsManager(
        IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<ExecutionResult<ProfileResponse>> AssignFollowingAsync(
        FollowingInfo followingInfo,
        CancellationToken cancellationToken = default)
    {
        var profileResponse =
            await _profileRepository.AddFollowingAsync(followingInfo);

        return GetResult(profileResponse);
    }

    public async Task<ExecutionResult<ProfileResponse>> DeAssignFollowingAsync(
        FollowingInfo followingInfo,
        CancellationToken cancellationToken = default)
    {
        var profileResponse =
            await _profileRepository.RemoveFollowingAsync(followingInfo);

        return GetResult(profileResponse);
    }

    private static ExecutionResult<ProfileResponse> GetResult(
        ProfileResponse? profileResponse)
    {
        if (profileResponse is null)
        {
            return new(profileResponse, HttpStatusCode.BadRequest);
        }

        return new(profileResponse);
    }
}
