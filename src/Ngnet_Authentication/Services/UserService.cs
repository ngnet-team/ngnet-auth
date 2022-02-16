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

        public int UsersCount => this.database.Users.Where(x => !x.IsDeleted).Count();

        public object GetAccounts<T>(string userId, int? count = null)
        {
            IQueryable<User> users = this.database.Users
                .Where(x => !x.IsDeleted);

            if (users.Count() == 0)
                return default(T[]);

            if (count != null)
                users = users.Take((int)count);

            if (userId != null)
            {
                return users.Where(x => x.Id == userId).To<T>().FirstOrDefault();
            }
            else
            {
                return users.To<T>().ToArray();
            }
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
            User user = this.GetUserById(userId);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);

            user.IsDeleted = true;
            user.DeletedOn = DateTime.UtcNow;
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Deleted);
        }

        public async Task<ServiceResponseModel> DeleteAccount(string userId) // PERMANENT deletion!
        {
            User user = this.GetUserById(userId, true);
            if (user == null)
                return new ServiceResponseModel(this.GetErrors().UserNotFound, null);

            await this.RemoveAllUserRelated(user);

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
                Key = ChangableType.Resetpassword.ToString(),
                New = newPassword,
            });

            if (this.response.Errors == null)
                this.response.RawData = newPassword;

            return this.response;
        }

        public async Task<ServiceResponseModel> Update(UpdateRequestModel model)
        {
            User user = this.GetUserById(model.Id);
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
            User user = this.GetUserById(model.Id);
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

        protected async Task RemoveAllUserRelated(User user)
        {
            //Entries
            IQueryable<Entry> entries = this.database.Entries.Where(x => x.UserId == user.Id);
            this.database.Entries.RemoveRange(entries);
            //Addresses
            string addressId = user?.AddressId;
            IQueryable<Address> addresses = this.database.Addresses.Where(x => x.Id == addressId);
            this.database.Addresses.RemoveRange(addresses);
            //Contacts
            string contactId = user?.ContactId;
            IQueryable<Contact> contacts = this.database.Contacts.Where(x => x.Id == contactId);
            this.database.Contacts.RemoveRange(contacts);
            //Rights
            IQueryable<RightsChange> rights = this.database.RightsChanges
                .Where(x => x.From == user.Id || x.To == user.Id);
            this.database.RightsChanges.RemoveRange(rights);

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
            else if (ChangableType.Resetpassword.ToString().Equals(this.Capitalize(model.Key)))
            {
                //No need to validate this
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
