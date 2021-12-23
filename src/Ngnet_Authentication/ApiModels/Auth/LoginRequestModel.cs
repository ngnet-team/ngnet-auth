using System.ComponentModel.DataAnnotations;

using Database.Models;
using Mapper;

namespace ApiModels.Auth
{
    public class LoginRequestModel : IMapTo<User>
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}