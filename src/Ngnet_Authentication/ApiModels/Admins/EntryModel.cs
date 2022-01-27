using AutoMapper;

using Database.Models;
using Mapper;
using System;

namespace ApiModels.Admins
{
    public class EntryModel : IMapFrom<Entry>
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public bool Login { get; set; }

        public string CreatedOn { get; set; }

        //public void CreateMappings(IProfileExpression configuration)
        //{
        //    configuration.CreateMap<Entry, EntryModel>()
        //        .ForMember(x => x.CreatedOn, opt => opt.MapFrom(x => x.CreatedOn.ToLongDateString()));
        //}
    }
}
