using AutoMapper;
using Common;
using Database.Models;
using Mapper;

namespace Services.Seeding
{
    public class UserSeederModel : IMapTo<User>
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        //public void CreateMappings(IProfileExpression configuration)
        //{
        //    configuration.CreateMap<UserSeederModel, User>()
        //        .ForMember(x => x.PasswordHash, opt => opt.MapFrom(x => Hash.CreatePassword(x.Password)));
        //}
    }
}
