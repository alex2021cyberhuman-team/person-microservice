using System.Threading;
using System.Threading.Tasks;

namespace Conduit.Person.BusinessLogic
{
    public interface IProfileViewer
    {
        Task<ExecutionResult<ProfileResponse>> ReturnProfile(
            FollowingInfo followingInfo,
            CancellationToken cancellationToken);
    }
}
