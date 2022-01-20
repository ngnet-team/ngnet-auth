using System.ComponentModel.DataAnnotations;

using Common;
using Database.Models;
using Mapper;

namespace ApiModels.Users
{
    public class UpdateRequestModel : IMapTo<User>
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        [Range(Global.AgeMin, Global.AgeMax)]
        public int? Age { get; set; }

        public bool IsDeleted { get; set; }
    }
}
