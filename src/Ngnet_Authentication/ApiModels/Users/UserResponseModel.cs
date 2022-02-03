using Database.Models;
using Mapper;

namespace ApiModels.Users
{
    public class UserResponseModel :  UserOptionalModel, IMapFrom<User>
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string CreatedOn { get; set; }
    }
}
