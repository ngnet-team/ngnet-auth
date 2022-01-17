using System.ComponentModel.DataAnnotations;
using Common;
using Database.Models;
using Mapper;

namespace ApiModels.Users
{
    public class UserRequestModel : IMapTo<User>
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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        [Range(Global.AgeMin, Global.AgeMax)]
        public int? Age { get; set; }
    }
}
