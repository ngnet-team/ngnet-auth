using System;
using System.Linq;
using System.Threading.Tasks;

using ApiModels.Users;
using Common;
using Common.Enums;
using Common.Json.Service;
using Database;
using Database.Models;
using Mapper;
using Services.Base;
using Services.Interfaces;

namespace Services
{
    public class UserService : AuthService, IUserService
    {
        public UserService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public override RoleTitle RoleTitle { get; set; } = RoleTitle.User;

        public T Profile<T>(string userId)
        {
            return this.database.Users
                .Where(x => x.Id == userId)
                .Where(x => !x.IsDeleted)
                .To<T>()
                .FirstOrDefault();
        }

        public async Task<ServiceResponseModel> Logout(string userId)
        {
            await this.AddEntry(new Entry()
            {
                UserId = userId,
                LoggedOut = DateTime.UtcNow
            });

            return new ServiceResponseModel(null, this.GetSuccessMsg().LoggedOut);
        }

        public async Task<ServiceResponseModel> DeleteAccount(string userId)
        {
            User user = this.database.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);

            user.IsDeleted = true;
            user.DeletedOn = DateTime.UtcNow;
            await this.database.SaveChangesAsync();
            
            return new ServiceResponseModel(null, this.GetSuccessMsg().Deleted);
        }

        public bool ValidEmail(UserChangeModel model, User user)
        {
            //Base change validator
            if (this.ValidChange(model, user.Id))
                return false;

            if (user.Email != model.Old)
                return false;

            if (this.EmailValidator(model.New))
                return false;

            return true;
        }

        public bool ValidPassword(UserChangeModel model, User user)
        {
            //Base change validator
            if (this.ValidChange(model, user.Id))
                return false;

            if (user.PasswordHash != Hash.CreatePassword(model.New))
                return false;

            return true;
        }
    }
}
