using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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

namespace Services
{
    public class AuthService : BaseService, IAuthService
    {
        public AuthService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public virtual RoleTitle RoleTitle { get; set; } = RoleTitle.Guest;

        public async Task<ServiceResponseModel> Register(RegisterRequestModel model)
        {
            //Equal passwords check
            if (model.Password != model.RepeatPassword)
                return new ServiceResponseModel(GetErrors().NotEqualPasswords, null);
            //Username exists
            User user = this.GetUserByUsername(model.Username);
            if (user != null)
                return new ServiceResponseModel(GetErrors().ExistingUserName, null);

            //TODO: Email validator

            //Get role User
            Role role = this.GetRoleByEnum(RoleTitle.User);
            if (role == null)
                return new ServiceResponseModel(GetErrors().InvalidRole, null);

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
                LoggedIn = DateTime.UtcNow
            });

            return new ServiceResponseModel(null, this.GetSuccessMsg().LoggedIn, user);
        }

        public string CreateJwtToken(JwtTokenModel tokenModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenModel.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, tokenModel.UserId),
                    new Claim(ClaimTypes.Name, tokenModel.Username),
                    new Claim(ClaimTypes.Role, tokenModel.RoleName)
                }),
                Expires = DateTime.UtcNow.AddDays(Global.JwtTokenExpires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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

            user.Experiences.Add(exp);
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }

        public async Task<ServiceResponseModel> Update<T>(T model)
        {
            User mappedModel = MappingFactory.Mapper.Map<User>(model);

            User user = this.GetUserById(mappedModel.Id);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);

            user = this.ModifyEntity(mappedModel, user);

            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated);
        }

        public User GetUserById(string id)
        {
            return this.database.Users
                .Where(x => x.Id == id)
                .Where(x => !x.IsDeleted)
                .FirstOrDefault();
        }

        public User GetUserByUsername(string username)
        {
            return this.database.Users
                .Where(x => x.Username == username)
                .Where(x => !x.IsDeleted)
                .FirstOrDefault();
        }

        public Role GetUserRole(User user)
        {
            return this.database.Roles.FirstOrDefault(x => x.Id == user.RoleId);
        }

        public Role GetRoleByString(string roleName)
        {
            RoleTitle roleTitle;
            bool valid = Enum.TryParse<RoleTitle>(this.Capitalize(roleName), out roleTitle);
            if (!valid)
                return null;

            return this.database.Roles.FirstOrDefault(x => x.Title == roleTitle);
        }

        public Role GetRoleByEnum(RoleTitle roleTitle)
        {
            return this.database.Roles.FirstOrDefault(x => x.Title == roleTitle);
        }

        // ------------------- Protected ------------------- 

        protected User ModifyEntity(User mappedModel, User user)
        {
            user.Email = mappedModel.Email == null ? user.Email : mappedModel.Email;
            user.PasswordHash = mappedModel.PasswordHash == null ? user.PasswordHash : mappedModel.PasswordHash;
            user.FirstName = mappedModel.FirstName == null ? user.FirstName : mappedModel.FirstName;
            user.LastName = mappedModel.LastName == null ? user.LastName : mappedModel.LastName;
            user.Gender = mappedModel.Gender == null ? user.Gender : mappedModel.Gender;
            user.Age = mappedModel.Age == null ? user.Age : mappedModel.Age;

            user.ModifiedOn = DateTime.UtcNow;
            user.IsDeleted = mappedModel.IsDeleted == true ? mappedModel.IsDeleted : user.IsDeleted;
            user.DeletedOn = mappedModel.IsDeleted == true ? DateTime.UtcNow : user.DeletedOn;

            return user;
        }

        protected bool ValidChange(ChangeModel model, string userId)
        {
            //Should be equal
            if (model.New != model.Old)
                return false;
            //User Not Exists
            User user = this.GetUserById(userId);
            if (user == null || user.IsDeleted)
                return false;

            return true;
        }

        protected bool EmailValidator(string emailAddress)
        {
            // ------- Local validation ------- 
            string pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"; //needs to be upgraded, copied from: regexr.com/3e48o
            var matching = Regex.IsMatch(emailAddress, pattern);
            if (!matching)
                return false;

            return true; // need valid send grid api key before code below...

            // ------- real email validation ------- 
            //EmailSenderModel model = new EmailSenderModel(this.Admin.Email, emailAddress);
            //Response response = await this.emailSenderService.EmailConfirmation(model);

            //if (response == null || !response.IsSuccessStatusCode)
            //{
            //    return this.GetErrors().InvalidEmail;
            //}

            //return null;
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
            if (this.RoleTitle == RoleTitle.Owner)
                return true;

            return (int)this.RoleTitle < (int)role.Title;
        }
    }
}
