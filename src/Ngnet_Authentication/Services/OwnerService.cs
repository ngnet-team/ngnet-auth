using System;
using System.Linq;
using System.Threading.Tasks;

using ApiModels.Owners;
using Common.Enums;
using Common.Json.Service;
using Database;
using Database.Models;
using Services.Base;
using Services.Interfaces;

namespace Services
{
    public class OwnerService : AdminService, IOwnerService
    {
        public OwnerService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public override RoleType RoleType { get; set; } = RoleType.Owner;

        public async Task<ServiceResponseModel> SetMaxRoles(RoleModel[] models)
        {
            foreach (var model in models)
            {
                RoleType roleType;
                bool validRole = Enum.TryParse<RoleType>(model.RoleName, out roleType);

                if (!validRole)
                    return new ServiceResponseModel(this.GetErrors().InvalidRole, null);

                Role role = this.database.Roles.FirstOrDefault(x => x.Type == roleType);
                if (role.MaxCount == model.MaxCount)
                {
                    continue;
                }

                int usersInThisRole = this.database.Users
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.RoleId == role.Id)
                    .Count();

                if (model.MaxCount < usersInThisRole)
                {
                    return new ServiceResponseModel(this.GetErrors().NoPermissions, null);
                }

                role.MaxCount = model.MaxCount;
            }

            await this.database.SaveChangesAsync();
            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }
    }
}
