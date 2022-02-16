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
            IQueryable<AdminResponseModel> users = this.database.Users.To<AdminResponseModel>();

            if (users.Count() == 0)
                return null;

            if (count != null)
                users = users.Take((int)count);

            foreach (var user in users)
            {
                user.RoleName = this.GetUserRoleType(user.Id)?.ToString();
            }

            return users.ToArray();
        }

        public RoleModel[] GetRoles()
        {
            IQueryable<RoleModel> roles = this.database.Roles
                .Select(x => new RoleModel()
                {
                    Id = x.Id,
                    Name = x.Type.ToString(),
                    MaxCount = x.MaxCount,
                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,
                    DeletedOn = x.DeletedOn,
                    IsDeleted = x.IsDeleted,
                });

            if (roles.Count() == 0)
                return null;

            return roles.ToArray();
        }

        public EntryModel[] GetEntries(string userId = null)
        {
            IQueryable<Entry> entries = this.database.Entries;

            if (userId != null)
                entries = entries.Where(x => x.UserId == userId);

            IQueryable<EntryModel> models = entries
                   .OrderByDescending(x => x.Id)
                   .Select(x => new EntryModel()
                   {
                       UserId = x.UserId,
                       Username = x.Username,
                       Login = x.Login,
                       CreatedOn = this.DateToString(x.CreatedOn),
                   });

            int res = models.Count();

            if (models.Count() == 0)
                return null;

            return models.ToArray();
        }

        public RightsChangeModel[] GetRightsChanges(string author = null)
        {
            IQueryable<RightsChange> rights = this.database.RightsChanges;

            if (author != null)
                rights = rights.Where(x => x.From == author);

            IQueryable<RightsChangeModel> models = rights.OrderByDescending(x => x.Id)
                   .Select(x => new RightsChangeModel()
                   {
                       From = x.From,
                       To = x.To,
                       Role = x.Role.ToString(),
                       Date = this.DateToString(x.Date),
                   });

            if (models.Count() == 0)
                return null;

            return models.ToArray();
        }
    }
}
