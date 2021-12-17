using AutoMapper;
using Common;
using Database.Models;
using Mapper;
using System.ComponentModel.DataAnnotations;

namespace ApiModels.Users
{
    public class UserRequestModel : IMapTo<User>
    {
        public string Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(6)]
        public string UserName { get; set; }

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
