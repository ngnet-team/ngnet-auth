using Database.Models;
using Mapper;

namespace ApiModels.Users
{
    public class UpdateRequestModel : UserOptionalModel, IMapTo<User>
    {
        public string Id { get; set; }
    }
}
