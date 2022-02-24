using Database.Models;
using Mapper;

namespace ApiModels.Guest
{
    public class LoginRequestModel : IMapTo<User>
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}