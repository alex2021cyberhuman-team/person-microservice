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
        var (profile, _) = await _profileRepository.FindAsync(followingInfo);

        if (profile is null)
        {
            return new(profile, HttpStatusCode.NotFound);
        }

        return new(profile);
    }
}
