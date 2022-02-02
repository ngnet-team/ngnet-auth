using ApiModels.Users;
using Database.Models;
using Mapper;

namespace Services.Seeding
{
    public class UserSeederModel : UserOptionalModel, IMapTo<User>
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
