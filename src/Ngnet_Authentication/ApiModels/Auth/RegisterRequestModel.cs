using AutoMapper;
using Common;
using Common.Enums;
using Database.Models;
using Mapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiModels.Auth
{
    public class RegisterRequestModel : IMapTo<User>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MinLength(6)]
        public string RepeatPassword { get; set; }

        public RoleTitle Role { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Age { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<RegisterRequestModel, User>()
                .ForMember(x => x.CreatedOn, opt => opt.MapFrom(x => DateTime.UtcNow))
                .ForMember(x => x.PasswordHash, opt => opt.MapFrom(x => Hash.CreatePassword(x.Password)));
        }
    }
}
