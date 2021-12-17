using AutoMapper;
using Database.Models;
using Mapper;

namespace ApiModels.Auth
{
    public class ExperienceModel : IMapFrom<UserExperience>
    {
        public string LoggedIn { get; set; }

        public string LoggedOut { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UserExperience, ExperienceModel>()
                .ForMember(x => x.LoggedIn, opt => opt.MapFrom(x => x.LoggedIn.GetValueOrDefault().ToShortDateString()))
                .ForMember(x => x.LoggedOut, opt => opt.MapFrom(x => x.LoggedOut.GetValueOrDefault().ToShortDateString()));
        }
    }
}
