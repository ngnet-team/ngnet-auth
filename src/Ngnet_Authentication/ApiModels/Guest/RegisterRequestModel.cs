using ApiModels.Users;
using Database.Models;
using Mapper;

namespace ApiModels.Guest
{
    public class RegisterRequestModel : UserOptionalModel, IMapTo<User>
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
