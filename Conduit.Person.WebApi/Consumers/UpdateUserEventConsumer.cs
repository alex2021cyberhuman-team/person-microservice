using Conduit.Person.BusinessLogic;
using Conduit.Shared.Events.Models.Users.Update;
using Conduit.Shared.Events.Services;

namespace Conduit.Person.WebApi.Consumers;

public class UpdateUserEventConsumer : IEventConsumer<UpdateUserEventModel>
{
    private readonly IProfileRepository _profileRepository;

    public UpdateUserEventConsumer(
        IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task ConsumeAsync(
        UpdateUserEventModel message)
    {
        await _profileRepository.SaveAfterUpdateEventAsync(message);
    }
}
