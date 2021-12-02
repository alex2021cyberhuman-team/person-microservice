using System.Threading;
using System.Threading.Tasks;

namespace Conduit.Person.BusinessLogic
{
    public interface IFollowingsManager
    {
        Task<ExecutionResult<Empty>> AssignFollowingAsync(
            FollowingInfo followingInfo,
            CancellationToken cancellationToken = default);

        Task<ExecutionResult<Empty>> DeAssignFollowingAsync(
            FollowingInfo followingInfo,
            CancellationToken cancellationToken = default);
    }
}
