using ApiModels.Auth;
using ApiModels.Users;
using AutoMapper;
using Common;
using Database.Models;
using Mapper;
using System.Collections.Generic;

namespace ApiModels.Admins
{
    public class AdminRequestModel : UserRequestModel, IMapTo<User>
    {
        public AdminRequestModel()
        {
            this.Experiances = new HashSet<ExperienceModel>();
        }

        public string RoleName { get; set; }

        public string CreatedOn { get; set; }

        public ICollection<ExperienceModel> Experiances { get; set; }

        public string ModifiedOn { get; set; }

        public string DeletedOn { get; set; }

        public bool IsDeleted { get; set; }

        public bool PermanentDeletion { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AdminRequestModel, User>()
                .ForMember(x => x.PasswordHash, opt => opt.MapFrom(x => Hash.CreatePassword(x.Password)));
        }
    }
}
