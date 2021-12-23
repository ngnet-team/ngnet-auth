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

        public async Task<ServiceResponseModel> SetRoleCounts(MaxRoles maxRoles)
        {
            (RoleType? roleType, int? count) = maxRoles.Get();
            //Nullable input
            if (roleType == null || count == null)
                return new ServiceResponseModel(this.GetErrors().InvalidRole, null);

            Role role = this.database.Roles.FirstOrDefault(x => x.Type == roleType);
            //New count should be different from current stored max count.
            if (role.MaxCount == count)
            {
                return new ServiceResponseModel(null, this.GetSuccessMsg().AlreadyStored);
            }
            //New count should be greater or equal to current users in that role.
            int usersInThisRole = this.database.Users.Where(x => !x.IsDeleted).Where(x => x.RoleId == role.Id).Count();
            if (count < usersInThisRole)
            {
                return new ServiceResponseModel(this.GetErrors().NoPermissions, null);
            }

            role.MaxCount = count;
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }
    }
}
