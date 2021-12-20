using Conduit.Shared.Events.Models.Users.Register;
using Conduit.Shared.Events.Models.Users.Update;

namespace Conduit.Person.BusinessLogic;

public interface IProfileRepository
{
    Task SaveAfterRegisterEventAsync(
        RegisterUserEventModel registerUserEventModel);

    Task SaveAfterUpdateEventAsync(
        UpdateUserEventModel updateUserEventModel);

    Task<(ProfileResponse?, Guid?)> FindAsync(
        FollowingInfo info);

    Task<(ProfileResponse?, Guid?)> AddFollowingAsync(
        FollowingInfo info);

    Task<(ProfileResponse?, Guid?)> RemoveFollowingAsync(
        FollowingInfo info);
}
