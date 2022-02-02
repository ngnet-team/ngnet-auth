using System.ComponentModel.DataAnnotations;

using Common;
using Database.Models;
using Mapper;
using ApiModels.Users;

namespace ApiModels.Auth
{
    public class RegisterRequestModel : UserOptionalModel, IMapTo<User>
    {
        [Required]
        [EmailAddress]
        [MinLength(Global.EmailMinLength), MaxLength(Global.EmailMaxLength)]
        public string Email { get; set; }

        [Required]
        [MinLength(Global.UsernameMinLength), MaxLength(Global.UsernameMaxLength)]
        public string Username { get; set; }

        [Required]
        [MinLength(Global.PasswordMinLength), MaxLength(Global.PasswordMaxLength)]
        public string Password { get; set; }

        [Required]
        [MinLength(Global.PasswordMinLength), MaxLength(Global.PasswordMaxLength)]
        public string RepeatPassword { get; set; }
    }
}
