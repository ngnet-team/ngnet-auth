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

namespace Services
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

            foreach (var user in users)
            {
                user.Experiances = this.GetEntries(user.Id);
            }

            return users;
        }

        public User GetDeletableUser(string userId)
        {
            return this.database.Users.FirstOrDefault(x => x.Id == userId);
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

        public ICollection<EntryModel> GetEntries(string UserId)
        {
            return this.database.Entries.Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.Id)
                .To<EntryModel>()
                //To avoid too many records in client
                .Take(20)
                .OrderByDescending(x => x.LoggedIn)
                .ThenByDescending(x => x.LoggedOut)
                .ToHashSet();
        }

        public async Task<ServiceResponseModel> DeleteUser(User user)
        {
            if (user == null)
                return new ServiceResponseModel(this.GetErrors().UserNotFound, null);

            await this.RemoveAllUserRelated(user.Id);

            this.database.Users.Remove(user);
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Deleted);
        }

        // ------------------ Private ------------------

        private async Task RemoveAllUserRelated(string userId)
        {
            //Entries:
            IQueryable<Entry> entries = this.database.Entries.Where(x => x.UserId == userId);
            this.database.Entries.RemoveRange(entries);

            await this.database.SaveChangesAsync();
        }
    }
}
