using AutoMapper;
using System.ComponentModel.DataAnnotations;

using Common;
using Common.Enums;
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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public int? Age { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UserRequestModel, User>()
                .ForMember(x => x.PasswordHash, opt => opt.MapFrom(x => Hash.CreatePassword(x.Password)));
        }
    }
}
