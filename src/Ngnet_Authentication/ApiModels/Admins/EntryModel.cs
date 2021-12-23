using AutoMapper;

using Database.Models;
using Mapper;

namespace ApiModels.Admins
{
    public class EntryModel : IMapFrom<Entry>
    {
        public string LoggedIn { get; set; }

        public string LoggedOut { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Entry, EntryModel>()
                .ForMember(x => x.LoggedIn, opt => opt.MapFrom(x => x.LoggedIn.GetValueOrDefault().ToShortDateString()))
                .ForMember(x => x.LoggedOut, opt => opt.MapFrom(x => x.LoggedOut.GetValueOrDefault().ToShortDateString()));
        }
    }
}
