using Database.Models;
using Mapper;

namespace ApiModels.Users
{
    public class UserResponseModel : IMapFrom<User>
    {
        public string Email { get; set; }

        public string UserName { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public int? Age { get; set; }
    }
}
