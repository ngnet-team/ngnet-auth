using AutoMapper;
using System.Collections.Generic;

using ApiModels.Users;
using Database.Models;

namespace ApiModels.Admins
{
    public class AdminResponseModel : UserResponseModel
    {
        public AdminResponseModel()
        {
            this.Entries = new HashSet<EntryModel>();
        }

        public string Id { get; set; }

        public string RoleName { get; set; }

        public string CreatedOn { get; set; }

        public ICollection<EntryModel> Entries { get; set; }

        public string ModifiedOn { get; set; }

        public string DeletedOn { get; set; }

        public bool IsDeleted { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<User, AdminResponseModel>()
                .ForMember(x => x.CreatedOn, opt => opt.MapFrom(x => x.CreatedOn.ToShortDateString()))
                .ForMember(x => x.ModifiedOn, opt => opt.MapFrom(x => x.ModifiedOn != null ? x.ModifiedOn.Value.ToShortDateString() : null))
                .ForMember(x => x.DeletedOn, opt => opt.MapFrom(x => x.DeletedOn != null ? x.DeletedOn.Value.ToShortDateString() : null));
        }
    }
}
