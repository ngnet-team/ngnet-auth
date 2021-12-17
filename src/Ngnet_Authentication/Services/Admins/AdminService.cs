using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Database;
using Database.Models;
using Mapper;
using Services.Auth;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Admins
{
    public class AdminService : AuthService, IAdminService
    {
        private readonly RoleTitle roleTitle = RoleTitle.Admin;

        public AdminService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public bool HasPermissions(AdminRequestModel model)
        {
            Role role = this.GetRoleByString(model.RoleName);
            return (int)role.Title < (int)this.roleTitle;
        }

        public async Task<CRUD> ChangeRole(AdminRequestModel model)
        {
            if (this.HasPermissions(model))
                return CRUD.NoPermissions;

            Role role = this.GetRoleByString(model.RoleName);
            if (role == null)
                return CRUD.NotFound;

            User user = this.GetUser(model.Id);
            if (user == null)
                return CRUD.NotFound;

            UserRole userRole = this.database.UserRoles.FirstOrDefault(x => x.UserId == user.Id);
            if (userRole == null)
                return CRUD.NotFound;

            this.database.UserRoles.Remove(userRole);
            await this.database.UserRoles.AddAsync(new UserRole()
            {
                User = user,
                Role = role,
            });

            await this.database.SaveChangesAsync();
            return CRUD.Created;
        }

        public AdminResponseModel[] GetUsers(int count = 10000)
        {
            var users = this.database.Users
            .To<AdminResponseModel>()
            .Take(count)
            .ToArray();

            foreach (var user in users)
            {
                user.RoleName = this.database.UserRoles.FirstOrDefault(x => x.UserId == x.Id).Role.Title.ToString();
                user.Experiances = this.GetExperiences(user.Id);
            }

            return users;
        }
    }
}
