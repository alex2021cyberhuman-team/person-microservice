using System.Net;

namespace Conduit.Person.BusinessLogic;

public class ProfileViewer : IProfileViewer
{
    private readonly IProfileRepository _profileRepository;

    public ProfileViewer(
        IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<ExecutionResult<ProfileResponse>> ReturnProfile(
        FollowingInfo followingInfo,
        CancellationToken cancellationToken)
    {
        var profileResponse = await _profileRepository.FindAsync(followingInfo);

        if (profileResponse is null)
        {
            return new(profileResponse, HttpStatusCode.NotFound);
        }

        return new(profileResponse);
    }
}
