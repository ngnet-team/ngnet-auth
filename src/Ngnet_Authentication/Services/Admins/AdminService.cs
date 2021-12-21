using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Database;
using Database.Models;
using Mapper;
using Services.Users;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Admins
{
    public class AdminService : UserService, IAdminService
    {
        public AdminService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public override RoleTitle RoleTitle { get; set; } = RoleTitle.Admin;

        public T GetUserIncludedDeleted<T>(string userId)
        {
            return this.database.Users
                .To<T>()
                .FirstOrDefault();
        }

        public async Task<ServiceResponseModel> ChangeRole(AdminRequestModel model)
        {
            if (this.HasPermissions(model.RoleName))
                return new ServiceResponseModel(GetErrors().NoPermissions, null);

            Role role = this.GetRoleByString(model.RoleName);
            if (role == null)
                return new ServiceResponseModel(GetErrors().InvalidRole, null);

            User user = this.GetUserIncludedDeleted<User>(model.Id);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);

            user.Role = role;
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }

        public AdminResponseModel[] GetUsers(int count = 10000)
        {
            var users = this.database.Users
            .To<AdminResponseModel>()
            .Take(count)
            .ToArray();

            foreach (var user in users)
            {
                user.Experiances = this.GetExperiences(user.Id);
            }

            return users;
        }
    }
}
