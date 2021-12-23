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

        public override RoleType RoleType { get; set; } = RoleType.User;

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

        public ServiceResponseModel Change(UserChangeModel model, User user)
        {
            ServiceResponseModel response = new ServiceResponseModel(null, null);
            //Changabe types
            if (ChangableType.Email.ToString().Equals(this.Capitalize(model.Key)))
            {
                if (!this.ValidEmail(model, user))
                    response.Errors = this.GetErrors().InvalidEmail;
                else
                    response.RawData = new UserRequestModel() { Email = model.New };
            }
            else if (ChangableType.Username.ToString().Equals(this.Capitalize(model.Key)))
            {
                if (!this.ValidUsername(model, user))
                    response.Errors = this.GetErrors().InvalidUsername;
                else
                    response.RawData = new UserRequestModel() { Username = model.New };
            }
            else if (ChangableType.Password.ToString().Equals(this.Capitalize(model.Key)))
            {
                if (!this.ValidPassword(model, user))
                    response.Errors = this.GetErrors().InvalidPassword;
                else
                    response.RawData = new UserRequestModel() { Password = model.New };
            }
            else
            {
                response.Errors = this.GetErrors().NoPermissions;
            }

            return response;
        }

        // ----------------------- Private -----------------------

        private bool ValidEmail(UserChangeModel model, User user)
        {
            //Base change validator
            if (!this.ValidChange(model, user))
                return false;
            //Length check
            if (model.New.Length < Global.EmailMinLength || Global.EmailMaxLength < model.New.Length)
                return false;
            //Current one is Not equal to old value 
            if (user.Email != model.Old)
                return false;
            //Current one is equal to new value
            if (user.Email == model.New)
                return false;
            //Additional validator
            if (!Global.EmailValidator(model.New))
                return false;

            return true;
        }

        private bool ValidUsername(UserChangeModel model, User user)
        {
            //Base change validator
            if (!this.ValidChange(model, user))
                return false;
            //Length check
            if (model.New.Length < Global.UsernameMinLength || Global.UsernameMaxLength < model.New.Length)
                return false;
            //Current one is Not equal to old value 
            if (user.Username != model.Old)
                return false;
            //Current one is equal to new value
            if (user.Username == model.New)
                return false;

            return true;
        }

        private bool ValidPassword(UserChangeModel model, User user)
        {
            //Base change validator
            if (!this.ValidChange(model, user))
                return false;
            //Length check
            if (model.New.Length < Global.PasswordMinLength || Global.PasswordMaxLength < model.New.Length)
                return false;
            //Current one is Not equal to old value 
            if (user.PasswordHash != Hash.CreatePassword(model.Old))
                return false;
            //Current one is equal to new value
            if (user.PasswordHash == Hash.CreatePassword(model.New))
                return false;

            return true;
        }
    }
}
