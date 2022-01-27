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

        public async Task<ServiceResponseModel> Logout(string userId, string username)
        {
            await this.AddEntry(new Entry()
            {
                UserId = userId,
                Username = username,
                Login = false,
                CreatedOn = DateTime.UtcNow
            });

            return new ServiceResponseModel(null, this.GetSuccessMsg().LoggedOut);
        }

        public async Task<ServiceResponseModel> Delete(string userId) // Marked as deleted ONLY!
        {
            User user = this.database.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);

            user.IsDeleted = true;
            user.DeletedOn = DateTime.UtcNow;
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Deleted);
        }

        public async Task<ServiceResponseModel> DeleteAccount(string userId) // PERMANENT deletion!
        {
            User user = this.GetUser(userId);
            if (user == null)
                return new ServiceResponseModel(this.GetErrors().UserNotFound, null);

            await this.RemoveAllUserRelated(user.Id);

            this.database.Users.Remove(user);
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Deleted);
        }

        public async Task<ServiceResponseModel> ResetPassword(string userId)
        {
            string newPassword = Global.CreateRandom;
            this.response = await this.Change(new ChangeRequestModel() 
            {
                Id = userId,
                Key = ChangableType.Password.ToString(),
                New = newPassword,
            });

            if (this.response.Errors == null)
                this.response.RawData = newPassword;

            return this.response;
        }

        public async Task<ServiceResponseModel> Update(UpdateRequestModel model)
        {
            User user = this.GetUser(model.Id);
            this.response = this.ValidUpdateModel(model, user);
            if (this.response.Errors != null)
                return this.response;

            user = this.ModifyEntity(user, model, null);
            if (user != null)
            {
                await this.database.SaveChangesAsync();
                return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
            }
            else
                return new ServiceResponseModel(null, null);
        }

        public async Task<ServiceResponseModel> Change(ChangeRequestModel model)
        {
            User user = this.GetUser(model.Id);
            this.response = this.ValidChangeModel(model, user);
            if (this.response.Errors != null)
                return this.response;

            user = this.ModifyEntity(user, null, model);
            if (user != null)
            {
                await this.database.SaveChangesAsync();
                return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
            }
            else
                return new ServiceResponseModel(null, null);
        }

        // ------------------ Protected ------------------

        protected async Task RemoveAllUserRelated(string userId)
        {
            //Entries:
            IQueryable<Entry> entries = this.database.Entries.Where(x => x.UserId == userId);
            this.database.Entries.RemoveRange(entries);

            await this.database.SaveChangesAsync();
        }

        // ----------------------- Private -----------------------

        private ServiceResponseModel ValidUpdateModel(UpdateRequestModel model, User user)
        {
            ServiceResponseModel response = new ServiceResponseModel(null, null);

            

            return response;
        }

        private ServiceResponseModel ValidChangeModel(ChangeRequestModel model, User user)
        {
            ServiceResponseModel response = new ServiceResponseModel(null, null);

            if (ChangableType.Email.ToString().Equals(this.Capitalize(model.Key)))
            {
                if (!this.ValidEmail(model, user))
                    response.Errors = this.GetErrors().InvalidEmail;
            }
            else if (ChangableType.Username.ToString().Equals(this.Capitalize(model.Key)))
            {
                if (!this.ValidUsername(model, user))
                    response.Errors = this.GetErrors().InvalidUsername;
            }
            else if (ChangableType.Password.ToString().Equals(this.Capitalize(model.Key)))
            {
                if (!this.ValidPassword(model, user))
                    response.Errors = this.GetErrors().InvalidPassword;
            }
            else
            {
                response.Errors = this.GetErrors().NoPermissions;
            }

            return response;
        }

        private bool ValidEmail(ChangeRequestModel model, User user)
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

        private bool ValidUsername(ChangeRequestModel model, User user)
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

        private bool ValidPassword(ChangeRequestModel model, User user)
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
