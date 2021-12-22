using Common.Enums;
using Common.Json.Service;
using Database;
using Services.Admins;
using System.Threading.Tasks;

namespace Services.Owners
{
    public class OwnerService : AdminService, IOwnerService
    {
        public OwnerService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public async Task<ServiceResponseModel> SetRoleMembers()
        {
            foreach (var user in this.database.Users)
            {
                RoleTitle roleTitle = this.GetUserRole(user);

            }

            return null;
        }
    }
}
