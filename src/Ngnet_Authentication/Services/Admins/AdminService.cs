﻿using ApiModels.Admins;
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

        public async Task<ServiceResponseModel> ChangeRole(AdminRequestModel model)
        {
            //Valid user check
            User user = this.GetUserById(model.Id);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);
            //Permissions check
            User changedUser = this.AddUserToRole(user, model.RoleName);
            if (changedUser == null)
                return new ServiceResponseModel(GetErrors().NoPermissions, null);

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

            foreach (var user in users)
            {
                user.Experiances = this.GetExperiences(user.Id);
            }

            return users;
        }

        public RoleResponseModel[] GetRoles(int? count = null)
        {
            var roles = this.database.Roles
                //Can't apply custom mapping, should be fixed!
                .Select(x => new RoleResponseModel() 
                { 
                    Id = x.Id,
                    Name = x.Title.ToString(),
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

            if (count != null)
                roles.Take((int)count);

            return roles;
        }
    }
}
