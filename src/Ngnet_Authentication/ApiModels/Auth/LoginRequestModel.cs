using Database.Models;
using Mapper;
using System.ComponentModel.DataAnnotations;

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