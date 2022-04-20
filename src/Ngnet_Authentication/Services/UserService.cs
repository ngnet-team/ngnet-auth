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
    public class UserService : GuestService, IUserService
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

        public UserOptionalModel IncludeComplicated(string userId)
        {
            User user = this.database.Users.FirstOrDefault(x => x.Id == userId);

            Address address = this.database.Addresses.FirstOrDefault(x => x.Id == user.AddressId);
            Contact contact = this.database.Contacts.FirstOrDefault(x => x.Id == user.ContactId);

            return new UserOptionalModel()
            {
                Address = new AddressRequestModel()
                {
                    Country = address?.Country,
                    City = address?.City,
                    Str = address?.Str,
                },
                Contact = new ContactRequestModel()
                {
                    Mobile = contact?.Mobile,
                    Email = contact?.Email,
                    Website = contact?.Website,
                    Facebook = contact?.Facebook,
                    Instagram = contact?.Instagram,
                    TikTok = contact?.TikTok,
                    Youtube = contact?.Youtube,
                    Twitter = contact?.Twitter,
                },
            };
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

        protected User AddUserToRole(User user, string roleName)
        {
            if (!this.RoomForRole(roleName))
                return null;

            user.RoleId = this.GetRole(roleName).Id;
            return user;
        }

        // ----------------------- Private -----------------------

        private bool RoomForRole(string roleName)
        {
            Role role = this.GetRole(roleName);
            if (role == null)
                return false;

            if (role?.MaxCount == null)
                return true;

            int usersInRole = this.database.Users
                .Where(x => !x.IsDeleted)
                .Where(x => x.RoleId == role.Id)
                .Count();

            return role.MaxCount > usersInRole;
        }

        private User ModifyEntity(User user, UpdateRequestModel updateModel, ChangeRequestModel changeModel)
        {
            //Updatable entities
            if (!Global.NullableObject(updateModel))
            {
                user.FirstName = updateModel.FirstName == null ? user.FirstName : updateModel.FirstName;

                user.MiddleName = updateModel.MiddleName == null ? user.MiddleName : updateModel.MiddleName;

                user.LastName = updateModel.LastName == null ? user.LastName : updateModel.LastName;

                GenderType gender;
                bool validGender = Enum.TryParse<GenderType>(this.Capitalize(updateModel.Gender), out gender);
                if (validGender)
                    user.Gender = updateModel.Gender == null ? user.Gender : gender;

                user.Age = updateModel.Age == null ? user.Age : updateModel.Age;

                if (!Global.NullableObject(updateModel?.Address))
                {
                    Address address = this.database.Addresses.FirstOrDefault(x => x.Id == user.AddressId);
                    address.Country = updateModel?.Address?.Country == null ? address.Country : updateModel?.Address?.Country;
                    address.City = updateModel?.Address?.City == null ? address.City : updateModel?.Address?.City;
                    address.Str = updateModel?.Address?.Str == null ? address.Str : updateModel?.Address?.Str;
                }

                if (!Global.NullableObject(updateModel?.Contact))
                {
                    Contact contact = this.database.Contacts.FirstOrDefault(x => x.Id == user.ContactId);
                    contact.Mobile = updateModel?.Contact?.Mobile == null ? contact.Mobile : updateModel?.Contact?.Mobile;
                    contact.Email = updateModel?.Contact?.Email == null ? contact.Email : updateModel?.Contact?.Email;
                    contact.Website = updateModel?.Contact?.Website == null ? contact.Website : updateModel?.Contact?.Website;
                    contact.Facebook = updateModel?.Contact?.Facebook == null ? contact.Facebook : updateModel?.Contact?.Facebook;
                    contact.Instagram = updateModel?.Contact?.Instagram == null ? contact.Instagram : updateModel?.Contact?.Instagram;
                    contact.TikTok = updateModel?.Contact?.TikTok == null ? contact.TikTok : updateModel?.Contact?.TikTok;
                    contact.Youtube = updateModel?.Contact?.Youtube == null ? contact.Youtube : updateModel?.Contact?.Youtube;
                    contact.Twitter = updateModel?.Contact?.Twitter == null ? contact.Twitter : updateModel?.Contact?.Twitter;
                }
            }
            //Changable entities
            else if (!Global.NullableObject(changeModel))
            {
                string key = this.Capitalize(changeModel.Key);

                if (ChangableType.Email.ToString().Equals(key))
                    user.Email = changeModel.New;

                else if (ChangableType.Username.ToString().Equals(key))
                    user.Username = changeModel.New;

                else if (ChangableType.Password.ToString().Equals(key))
                    user.PasswordHash = Hash.CreatePassword(changeModel.New);

                else
                    return null;
            }
            else
                return null;

            user.ModifiedOn = DateTime.UtcNow;

            return user;
        }

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
                if (!this.ValidExistingEmail(model, user))
                    response.Errors = this.GetErrors().InvalidEmail;
            }
            else if (ChangableType.Username.ToString().Equals(this.Capitalize(model.Key)))
            {
                if (!this.ValidExistingUsername(model, user))
                    response.Errors = this.GetErrors().InvalidUsername;
            }
            else if (ChangableType.Password.ToString().Equals(this.Capitalize(model.Key)))
            {
                if (!this.ValidExistingPassword(model, user))
                    response.Errors = this.GetErrors().InvalidPassword;
            }
            else
            {
                response.Errors = this.GetErrors().NoPermissions;
            }

            return response;
        }

        private bool ValidExistingEmail(ChangeRequestModel model, User user)
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
            //if (!Global.EmailValidator(model.New))
                //return false;

            return true;
        }

        private bool ValidExistingUsername(ChangeRequestModel model, User user)
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

        private bool ValidExistingPassword(ChangeRequestModel model, User user)
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

        private bool ValidChange(ChangeRequestModel model, User user)
        {
            //User Not Exists
            if (user == null || user.IsDeleted)
                return false;

            return true;
        }
    }
}
