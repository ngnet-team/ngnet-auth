using System.Linq;
using System.Threading.Tasks;

using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Database;
using Database.Models;
using Mapper;
using Services.Base;
using Services.Interfaces;
using System;

namespace Services
{
    public class AdminService : MemberService, IAdminService
    {
        public AdminService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public override RoleType RoleType { get; set; } = RoleType.Admin;

        public async Task<ServiceResponseModel> ChangeRole(AdminRequestModel model, string currUser)
        {
            //Valid user check
            User user = this.GetUserById(model.Id, true);
            if (user == null)
                return new ServiceResponseModel(this.GetErrors().UserNotFound, null);
            //Permissions check
            User changedUser = this.AddUserToRole(user, model.RoleName);
            if (changedUser == null)
                return new ServiceResponseModel(this.GetErrors().NoPermissions, null);

            user = changedUser;

            //add to rights changes
            RoleType roleType = Enum.Parse<RoleType>(this.Capitalize(model.RoleName));
            RightsChange rights = new RightsChange()
            {
                From = currUser,
                To = model.Id,
                Role = roleType,
                Date = DateTime.UtcNow,
            };
            await this.database.RightsChanges.AddAsync(rights);

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
                user.RoleName = this.GetUserRoleType(user.Id)?.ToString();
            }

            return users;
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
                       CreatedOn = this.DateToString(x.CreatedOn),
                   })
                   .ToArray();
        }

        public RightsChangeModel[] GetRightsChanges(RightsChangeModel model = null)
        {
            var rightsChanges = this.database.RightsChanges;

            if (model?.From != null)
            {
                rightsChanges.Where(x => x.From == model.From);
            }

            if (model?.To != null)
            {
                rightsChanges.Where(x => x.To == model.To);
            }

            if (model?.Role != null)
            {
                RoleType? roleType = this.GetRoleType(model?.Role);
                rightsChanges.Where(x => x.Role == roleType);
            }

            return rightsChanges
                   .OrderByDescending(x => x.Id)
                   .Select(x => new RightsChangeModel()
                   {
                       From = x.From,
                       To = x.To,
                       Role = x.Role.ToString(),
                       Date = this.DateToString(x.Date),
                   })
                   .ToArray();
        }
    }
}
