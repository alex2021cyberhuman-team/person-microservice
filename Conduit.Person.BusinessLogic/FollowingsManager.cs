using System.Net;
using Conduit.Shared.Events.Models.Profiles.CreateFollowing;
using Conduit.Shared.Events.Models.Profiles.RemoveFollowing;
using Conduit.Shared.Events.Services;

namespace Conduit.Person.BusinessLogic;

public class FollowingsManager : IFollowingsManager
{
    private readonly IEventProducer<CreateFollowingEventModel>
        _createFollowingProducer;

    private readonly IProfileRepository _profileRepository;

    private readonly IEventProducer<RemoveFollowingEventModel>
        _removeFollowingProducer;

    public FollowingsManager(
        IProfileRepository profileRepository,
        IEventProducer<CreateFollowingEventModel> createFollowingProducer,
        IEventProducer<RemoveFollowingEventModel> removeFollowingProducer)
    {
        _profileRepository = profileRepository;
        _createFollowingProducer = createFollowingProducer;
        _removeFollowingProducer = removeFollowingProducer;
    }

    public async Task<ExecutionResult<ProfileResponse>> AssignFollowingAsync(
        FollowingInfo followingInfo,
        CancellationToken cancellationToken = default)
    {
        var (profile, _) = await _profileRepository.FindAsync(followingInfo);
        if (profile is null || profile.Profile.Following)
        {
            return GetResult(null);
        }

        var (newFollowedProfile, followedId) =
            await _profileRepository.AddFollowingAsync(followingInfo);

        await _createFollowingProducer.ProduceEventAsync(new()
        {
            FollowedId = followedId!.Value,
            FollowerId = followingInfo.FollowerUserId!.Value
        });

        return GetResult(newFollowedProfile);
    }

    public async Task<ExecutionResult<ProfileResponse>> DeAssignFollowingAsync(
        FollowingInfo followingInfo,
        CancellationToken cancellationToken = default)
    {
        var (profile, _) = await _profileRepository.FindAsync(followingInfo);
        if (profile is null || profile.Profile.Following == false)
        {
            return GetResult(null);
        }

        var (newFollowedProfile, followedId) =
            await _profileRepository.RemoveFollowingAsync(followingInfo);

        await _removeFollowingProducer.ProduceEventAsync(new()
        {
            FollowedId = followedId!.Value,
            FollowerId = followingInfo.FollowerUserId!.Value
        });

        return GetResult(newFollowedProfile);
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
