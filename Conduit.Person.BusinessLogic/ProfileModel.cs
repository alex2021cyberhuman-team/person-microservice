using System.ComponentModel.DataAnnotations;

namespace Conduit.Person.BusinessLogic
{
    public record ProfileModel
    {
        public ProfileModel(
            string username, 
            string? image = null,
            string? bio = null, 
            bool following = false)
        {
            Username = username;
            Image = image;
            Bio = bio;
            Following = following;
        }

        [Required]
        public string Username { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? Image { get; init; }

        [DataType(DataType.MultilineText)]
        public string? Bio { get; init; }

        [Required]
        public bool Following { get; set; }
    }
}