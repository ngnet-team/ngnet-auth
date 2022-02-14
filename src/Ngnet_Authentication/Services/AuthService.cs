﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

using ApiModels.Auth;
using Common;
using Common.Enums;
using Common.Json.Service;
using Database;
using Database.Models;
using Mapper;
using Services.Base;
using Services.Interfaces;
using ApiModels.Dtos;
using ApiModels.Users;

namespace Services
{
    public class AuthService : BaseService, IAuthService
    {
        public AuthService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public virtual RoleType RoleType { get; set; } = RoleType.Auth;

        public async Task<ServiceResponseModel> Register(RegisterRequestModel model)
        {
            if (!this.ValidPassword(model))
                return new ServiceResponseModel(this.GetErrors().NotEqualPasswords, null);

            if (!this.ValidNames(model))
                return new ServiceResponseModel(this.GetErrors().InvalidName, null);

            if (!this.ValidUsername(model))
                return new ServiceResponseModel(this.GetErrors().ExistingUserName, null);

            if (!this.ValidEmail(model))
                return new ServiceResponseModel(this.GetErrors().InvalidEmail, null);

            //Get role User
            Role role = this.GetRoleByEnum(RoleType.User);
            if (role == null)
                return new ServiceResponseModel(this.GetErrors().InvalidRole, null);

            Address address = new Address()
            {
                Country = model?.Address?.Country,
                City = model?.Address?.City,
                Str = model?.Address?.Str,
            };
            await this.database.Addresses.AddAsync(address);

            Contact contact = new Contact();
            await this.database.Contacts.AddAsync(contact);

            User user = new User()
            {
                RoleId = role.Id,
                Email = model.Email,
                Username = model.Username,
                PasswordHash = Hash.CreatePassword(model?.Password),
                //Optional
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Gender = Global.GetGender(model?.Gender),
                Age = model?.Age,
                AddressId = address.Id,
                ContactId = contact.Id,
            };
            await this.database.Users.AddAsync(user);

            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Registered);
        }

        public async Task<ServiceResponseModel> Login(LoginRequestModel model)
        {
            //Username does Not exist
            User user = this.GetUserByUsername(model.Username);
            if (user == null)
                return new ServiceResponseModel(GetErrors().InvalidUsername, null);
            //Invalid password
            string hashedPassword = Hash.CreatePassword(model.Password);
            if (user.PasswordHash != hashedPassword)
                return new ServiceResponseModel(GetErrors().InvalidPassword, null);

            await this.AddEntry(new Entry()
            {
                UserId = user.Id,
                Username = user.Username,
                Login = true,
                CreatedOn = DateTime.UtcNow
            });

            UserDto userDto = MappingFactory.Mapper.Map<UserDto>(user);
            return new ServiceResponseModel(null, this.GetSuccessMsg().LoggedIn, userDto);
        }

        public string CreateJwtToken(JwtTokenModel tokenModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(tokenModel.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = tokenModel.SecretKey,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, tokenModel.UserId),
                    new Claim(ClaimTypes.Name, tokenModel.Username),
                    new Claim(ClaimTypes.Role, tokenModel.RoleName),
                }),
                Expires = DateTime.UtcNow.AddDays(Global.Constants.TokenExpires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }

        public async Task<ServiceResponseModel> AddEntry(Entry exp)
        {
            User user = this.GetUserById(exp.UserId);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);

            user.Entries.Add(exp);
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }

        public UserDto GetUserDtoById(string id, bool allowDeleted = false)
        {
            IQueryable<User> user = this.database.Users
                .Where(x => x.Id == id);

            if (!allowDeleted)
                user.Where(x => !x.IsDeleted);

            return user
                .To<UserDto>()
                .FirstOrDefault();
        }

        public UserDto GetUserDtoByUsername(string username, bool allowDeleted = false)
        {
            IQueryable<User> user = this.database.Users
                .Where(x => x.Username == username);

            if (!allowDeleted)
                user.Where(x => !x.IsDeleted);

            return user
                .To<UserDto>()
                .FirstOrDefault();
        }

        public Role GetUserRole(string userId)
        {
            User user = this.database.Users
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
                return null;

            return this.database.Roles
                .FirstOrDefault(x => x.Id == user.RoleId);
        }

        public Role GetRoleByString(string roleName)
        {
            RoleType roleType;
            bool valid = Enum.TryParse<RoleType>(this.Capitalize(roleName), out roleType);
            if (!valid)
                return null;

            return this.database.Roles.FirstOrDefault(x => x.Type == roleType);
        }

        public Role GetRoleByEnum(RoleType roleType)
        {
            return this.database.Roles.FirstOrDefault(x => x.Type == roleType);
        }

        // ------------------- Protected ------------------- 

        protected User GetUserById(string id, bool allowDeleted = false)
        {
            IQueryable<User> user = this.database.Users
                .Where(x => x.Id == id);

            if (!allowDeleted)
                user.Where(x => !x.IsDeleted);

            return user.FirstOrDefault();
        }

        protected User GetUserByUsername(string username, bool allowDeleted = false)
        {
            IQueryable<User> user = this.database.Users
                .Where(x => x.Username == username);

            if (!allowDeleted)
                user.Where(x => !x.IsDeleted);

            return user.FirstOrDefault();
        }

        protected User ModifyEntity(User user, UpdateRequestModel updateModel, ChangeRequestModel changeModel)
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

        protected bool ValidChange(ChangeRequestModel model, User user)
        {
            //User Not Exists
            if (user == null || user.IsDeleted)
                return false;

            return true;
        }

        protected User AddUserToRole(User user, string roleName)
        {
            //There's no room for more roles.
            if (!this.CanAddRole(roleName))
                return null;

            Role role = this.GetRoleByString(roleName);
            user.RoleId = role.Id;
            return user;
        }

        // ------------------- Private ------------------- 

        private bool CanAddRole(string roleName)
        {
            Role role = this.GetRoleByString(roleName);
            if (role == null)
                return false;

            if (role?.MaxCount == null)
                return true;

            int usersInRole = this.database.Users
                .Where(x => !x.IsDeleted)
                .Where(x => x.RoleId == role.Id)
                .Count();

            if (role.MaxCount <= usersInRole)
                return false;

            return true;
        }

        // ------ Request Validations ------

        private bool ValidUsername(RegisterRequestModel model)
        {
            User user = this.GetUserByUsername(model.Username);
            if (user != null)
                return false;

            return true;
        }

        private bool ValidPassword(RegisterRequestModel model)
        {
            //Add password validation
            return true;
        }

        private bool ValidEmail(RegisterRequestModel model)
        {
            //TODO: Email validator
            return true;
        }

        private bool ValidNames(RegisterRequestModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                if (model.FirstName.Length < Global.NameMinLength ||
                    Global.NameMaxLength < model.FirstName.Length)
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                if (model.LastName.Length < Global.NameMinLength ||
                    Global.NameMaxLength < model.LastName.Length)
                    return false;
            }

            return true;
        }
    }
}
