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

namespace Services
{
    public class AuthService : BaseService, IAuthService
    {
        public AuthService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public virtual RoleType RoleType { get; set; } = RoleType.Guest;

        public async Task<ServiceResponseModel> Register(RegisterRequestModel model)
        {
            //Equal passwords check
            if (model.Password != model.RepeatPassword)
                return new ServiceResponseModel(this.GetErrors().NotEqualPasswords, null);
            //Username exists
            User user = this.GetUser(null, model.Username);
            if (user != null)
                return new ServiceResponseModel(this.GetErrors().ExistingUserName, null);

            //TODO: Email validator

            //Get role User
            Role role = this.GetRoleByEnum(RoleType.User);
            if (role == null)
                return new ServiceResponseModel(this.GetErrors().InvalidRole, null);

            user = MappingFactory.Mapper.Map<User>(model);
            //Should be auto mapped
            user.RoleId = role.Id;
            user.PasswordHash = Hash.CreatePassword(model.Password);

            await this.database.Users.AddAsync(user);
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Registered);
        }

        public async Task<ServiceResponseModel> Login(LoginRequestModel model)
        {
            //Username does Not exist
            User user = this.GetUser(null, model.Username);
            if (user == null)
                return new ServiceResponseModel(GetErrors().InvalidUsername, null);
            //Invalid password
            string hashedPassword = Hash.CreatePassword(model.Password);
            if (user.PasswordHash != hashedPassword)
                return new ServiceResponseModel(GetErrors().InvalidPassword, null);

            await this.AddEntry(new Entry()
            {
                UserId = user.Id,
                LoggedIn = DateTime.UtcNow
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
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, tokenModel.UserId),
                    new Claim(ClaimTypes.Name, tokenModel.Username),
                    new Claim(ClaimTypes.Role, tokenModel.RoleName),
                }),
                Expires = DateTime.UtcNow.AddDays(Global.JwtTokenExpires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }

        public async Task<ServiceResponseModel> AddEntry(Entry exp)
        {
            User user = this.GetUser(exp.UserId);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);

            user.Experiences.Add(exp);
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }

        public async Task<ServiceResponseModel> Update<T>(T model)
        {
            User mappedModel = MappingFactory.Mapper.Map<User>(model); // TODO: If it's password update this doesn't work because of auto mapped Password to Password Hash

            User user = this.GetUser(mappedModel.Id);
            if (user == null)
                return new ServiceResponseModel(this.GetErrors().UserNotFound, null);

            user = this.ModifyEntity(mappedModel, user);

            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }

        public UserDto GetUserById(string id)
        {
            return this.database.Users
                .Where(x => x.Id == id)
                .Where(x => !x.IsDeleted)
                .To<UserDto>()
                .FirstOrDefault();
        }

        public UserDto GetUserByUsername(string username)
        {
            return this.database.Users
                .Where(x => x.Username == username)
                .Where(x => !x.IsDeleted)
                .To<UserDto>()
                .FirstOrDefault();
        }

        public Role GetUserRole(UserDto userDto)
        {
            User user = this.database.Users
                .FirstOrDefault(x => x.Id == userDto.Id);

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

        protected User GetUser(string id, string username = null)
        {
            var users = this.database.Users
                .Where(x => !x.IsDeleted);

            if (id != null)
                users = users.Where(x => x.Id == id);
            else
                users = users.Where(x => x.Username == username);

            return users.FirstOrDefault();
        }

        protected User ModifyEntity(User mappedModel, User user)
        {
            //Changable
            user.Email = mappedModel.Email == null ? user.Email : mappedModel.Email;
            user.PasswordHash = mappedModel.PasswordHash == null ? user.PasswordHash : mappedModel.PasswordHash;
            user.Username = mappedModel.Username == null ? user.Username : mappedModel.Username;
            //Updatable
            user.FirstName = mappedModel.FirstName == null ? user.FirstName : mappedModel.FirstName;
            user.LastName = mappedModel.LastName == null ? user.LastName : mappedModel.LastName;
            user.Gender = mappedModel.Gender == null ? user.Gender : mappedModel.Gender;
            user.Age = mappedModel.Age == null ? user.Age : mappedModel.Age;
            user.IsDeleted = mappedModel.IsDeleted == true ? mappedModel.IsDeleted : user.IsDeleted;
            //Auto updated
            user.ModifiedOn = DateTime.UtcNow;
            user.DeletedOn = mappedModel.IsDeleted == true ? DateTime.UtcNow : user.DeletedOn;

            return user;
        }

        protected bool ValidChange(ChangeModel model, User user)
        {
            //Both new ones should be equal
            if (model.New != model.RepeatNew)
                return false;
            //User Not Exists
            if (user == null || user.IsDeleted)
                return false;

            return true;
        }

        protected User AddUserToRole(User user, string roleName)
        {
            //There's room for more roles or have permissions to do it.
            if (this.CanAddRole(roleName) && this.HasPermissions(roleName))
            {
                Role role = this.GetRoleByString(roleName);
                user.RoleId = role.Id;
                return user;
            }

            return null;
        }

        // ------------------- Private ------------------- 

        private bool CanAddRole(string roleName)
        {
            Role role = this.GetRoleByString(roleName);
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

        private bool HasPermissions(string roleName)
        {
            Role role = this.GetRoleByString(roleName);
            if (role == null)
                return false;
            if (this.RoleType == RoleType.Owner)
                return true;

            return (int)this.RoleType < (int)role.Type;
        }
    }
}