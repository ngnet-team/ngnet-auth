using System.ComponentModel.DataAnnotations;
using Database.Models;
using Mapper;

namespace ApiModels.Users
{
    public class UserRequestModel : UserOptionalModel, IMapTo<User>
    {
        public string Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(6)]
        public string Username { get; set; }

        [MinLength(6)]
        public string Password { get; set; }

        //Internal purposes
        public string PasswordHash { get; set; }
    }
}
