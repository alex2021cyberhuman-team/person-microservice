namespace Conduit.Person.BusinessLogic
{
    public record ProfileResponse
    {
        public ProfileResponse(
            string username,
            string? image = null,
            string? bio = null,
            bool following = false)
        {
            Profile = new(
                username,
                image,
                bio,
                following);
        }

        public ProfileModel Profile { get; init; }
    }
}
