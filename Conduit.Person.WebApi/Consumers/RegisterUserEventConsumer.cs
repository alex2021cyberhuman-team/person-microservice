using Conduit.Person.BusinessLogic;
using Conduit.Shared.Events.Models.Users.Register;
using Conduit.Shared.Events.Services;

namespace Conduit.Person.WebApi.Consumers;

public class RegisterUserEventConsumer : IEventConsumer<RegisterUserEventModel>
{
    private readonly IProfileRepository _profileRepository;

    public RegisterUserEventConsumer(
        IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task ConsumeAsync(
        RegisterUserEventModel message)
    {
        await _profileRepository.SaveAfterRegisterEventAsync(message);
    }
}
