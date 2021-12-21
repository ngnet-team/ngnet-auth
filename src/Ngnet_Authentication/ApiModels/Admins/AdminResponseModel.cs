using ApiModels.Auth;
using ApiModels.Users;
using AutoMapper;
using Database.Models;
using Mapper;
using System.Collections.Generic;

namespace ApiModels.Admins
{
    public class AdminResponseModel : UserResponseModel, IMapFrom<User>
    {
        public AdminResponseModel()
        {
            this.Experiances = new HashSet<ExperienceModel>();
        }

        public string Id { get; set; }

        public string RoleName { get; set; }

        public string CreatedOn { get; set; }

        public ICollection<ExperienceModel> Experiances { get; set; }

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
