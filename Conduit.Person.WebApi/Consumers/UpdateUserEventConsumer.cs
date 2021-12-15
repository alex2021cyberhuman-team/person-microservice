using System.Text.Json;
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
        Console.WriteLine("UpdateUserEventModel Queue is working!!! {0}",
            JsonSerializer.Serialize(message));
        await _profileRepository.SaveAfterUpdateEventAsync(message);
    }
}
