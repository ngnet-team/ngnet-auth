using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

using Common;
using Database.Models;
using Mapper;

namespace ApiModels.Auth
{
    public class RegisterRequestModel : IMapTo<User>
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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        [Range(Global.AgeMin, Global.AgeMax)]
        public int? Age { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<RegisterRequestModel, User>()
                .ForMember(x => x.PasswordHash, opt => opt.MapFrom(x => Hash.CreatePassword(x.Password)))// Don't work?
                .ForMember(x => x.CreatedOn, opt => opt.MapFrom(x => DateTime.UtcNow));
        }
    }
}
