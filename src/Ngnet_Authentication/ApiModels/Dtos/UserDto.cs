
using Database.Models;
using Mapper;

namespace ApiModels.Dtos
{
    public class UserDto : IMapFrom<User>
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string RoleId { get; set; }
    }
}
