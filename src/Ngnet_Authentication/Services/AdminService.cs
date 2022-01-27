using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Database;
using Database.Models;
using Mapper;
using Services.Base;
using Services.Interfaces;
using ApiModels.Dtos;

namespace Services
{
    public class AdminService : MemberService, IAdminService
    {
        public AdminService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public override RoleType RoleType { get; set; } = RoleType.Admin;

        public async Task<ServiceResponseModel> ChangeRole(AdminRequestModel model)
        {
            //Valid user check
            User user = this.GetUser(model.Id);
            if (user == null)
                return new ServiceResponseModel(this.GetErrors().UserNotFound, null);
            //Permissions check
            User changedUser = this.AddUserToRole(user, model.RoleName);
            if (changedUser == null)
                return new ServiceResponseModel(this.GetErrors().NoPermissions, null);

            user = changedUser;
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }

        public AdminResponseModel[] GetUsers(int? count = null)
        {
            var users = this.database.Users
            .To<AdminResponseModel>()
            .ToArray();

            if (count != null)
                users.Take((int)count);
            //Add entries and role names
            foreach (var user in users)
            {
                user.Entries = this.GetEntries(user.Id);
                user.RoleName = this.GetUserRole(new UserDto()
                {
                    Id = user.Id
                }).Type.ToString();
            }

            return users;
        }

        public UserDto GetDeletableUser(string userId)
        {
            return this.database.Users
                .Where(x => x.Id == userId)
                .To<UserDto>()
                .FirstOrDefault(x => x.Id == userId);
        }

        public RoleModel[] GetRoles()
        {
            var roles = this.database.Roles
                //Can't apply custom mapping, should be fixed!
                .Select(x => new RoleModel()
                {
                    Id = x.Id,
                    Name = x.Type.ToString(),
                    MaxCount = x.MaxCount,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                    DeletedOn = x.DeletedOn,
                    IsDeleted = x.IsDeleted,
                })
                .ToArray();

            //var roles = this.database.Roles
            //    .To<RoleResponseModel>()
            //    .ToArray();

            return roles;
        }

        public EntryModel[] GetEntries(string userId = null)
        {
            var entries = this.database.Entries;

            if (userId != null)
                entries.Where(x => x.UserId == userId).Take(20);//To avoid too many records for single user

            return entries
                   .OrderByDescending(x => x.Id)
                   .Select(x => new EntryModel() 
                   {
                       UserId = x.UserId,
                       Username = x.Username,
                       Login = x.Login,
                       CreatedOn = x.CreatedOn.ToShortDateString() + " " + x.CreatedOn.ToLongTimeString(),
                   })
                   .ToArray();
        }
    }
}
