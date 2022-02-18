using Database.Models;
using Mapper;

namespace ApiModels.Auth
{
    public class LoginRequestModel : IMapTo<User>
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}