using System;
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
using Common.Json.Models;

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
            this.response = this.ValidNewUsername(model.Username);
            if (this.response?.Errors != null)
                return this.response;

            this.response = this.ValidNewPassword(model.Password);
            if (this.response?.Errors != null)
                return this.response;

            this.response = this.ValidNewEmail(model.Email);
            if (this.response?.Errors != null)
                return this.response;

            this.response = this.ValidNewNames(model);
            if (this.response?.Errors != null)
                return this.response;

            Role role = this.GetRole(RoleType.User.ToString());
            if (role == null)
                return new ServiceResponseModel(this.GetErrors().InvalidRole, null);

            Address address = new Address()
            {
                Country = model?.Address?.Country,
                City = model?.Address?.City,
                Str = model?.Address?.Str,
            };
            await this.database.Addresses.AddAsync(address);

            Contact contact = new Contact()
            {
                Mobile = model?.Contact?.Mobile,
                Email = model?.Contact?.Email,
                Website = model?.Contact?.Website,
                Facebook = model?.Contact?.Facebook,
                Instagram = model?.Contact?.Instagram,
                TikTok = model?.Contact?.TikTok,
                Youtube = model?.Contact?.Youtube,
                Twitter = model?.Contact?.Twitter,
            };
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
            //Required fields
            if (string.IsNullOrWhiteSpace(model.Username))
                return new ServiceResponseModel(this.GetErrors().RequiredUsername, null);
            if (string.IsNullOrWhiteSpace(model.Password))
                return new ServiceResponseModel(this.GetErrors().RequiredPassword, null);
            //User not found
            User user = this.GetUserByUsername(model.Username);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);
            //Invalid password
            string passwordHash = Hash.CreatePassword(model.Password);
            if (user.PasswordHash != passwordHash)
                return new ServiceResponseModel(GetErrors().InvalidPassword, null);
            //Success
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

        public async Task<ServiceResponseModel> ResetPassword(string email)
        {
            User user = this.database.Users
                .Where(x => !x.IsDeleted)
                .FirstOrDefault(x => x.Email == email);

            if (user == null)
                return new ServiceResponseModel(this.GetErrors().UserNotFound, null);

            string newPassword = Global.CreateRandom;
            user.PasswordHash = Hash.CreatePassword(newPassword);

            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Updated, newPassword);
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

        public RoleType? GetUserRoleType(string userId)
        {
            User user = this.database.Users
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
                return null;

            return this.database.Roles
                .FirstOrDefault(x => x.Id == user.RoleId)
                ?.Type;
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

        protected Role GetRole(string roleName)
        {
            RoleType? roleType = this.GetRoleType(roleName);
            return this.database.Roles.FirstOrDefault(x => x.Type == roleType);
        }

        protected RoleType? GetRoleType(string roleName)
        {
            RoleType roleType;
            bool valid = Enum.TryParse<RoleType>(this.Capitalize(roleName), out roleType);
            if (!valid)
                return null;

            return roleType;
        }

        // ------ Validations ------

        private ServiceResponseModel ValidNewUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return new ServiceResponseModel(this.GetErrors().RequiredUsername, null);

            if (username?.Length < Global.UsernameMinLength || Global.UsernameMaxLength < username?.Length)
            {
                ResponseMessage error = this.GetErrors().InvalidUsernameLength;
                string additional = $"{Global.UsernameMinLength}-{Global.UsernameMaxLength}";
                error.En += additional; error.Bg += additional;
                return new ServiceResponseModel(error, null);
            }

            User user = this.GetUserByUsername(username);
            if (user != null)
                return new ServiceResponseModel(this.GetErrors().ExistingUserName, null);

            return null;
        }

        private ServiceResponseModel ValidNewPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return new ServiceResponseModel(this.GetErrors().RequiredPassword, null);

            if (password?.Length < Global.PasswordMinLength || Global.PasswordMaxLength < password?.Length)
            {
                ResponseMessage error = this.GetErrors().InvalidPasswordLength;
                string additional = $"{Global.PasswordMinLength}-{Global.PasswordMaxLength}";
                error.En += additional; error.Bg += additional;
                return new ServiceResponseModel(error, null);
            }

            return null;
        }

        private ServiceResponseModel ValidNewEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new ServiceResponseModel(this.GetErrors().RequiredEmail, null);

            if (email?.Length < Global.EmailMinLength || Global.EmailMaxLength < email?.Length)
            {
                ResponseMessage error = this.GetErrors().InvalidEmailLength;
                string additional = $"{Global.EmailMinLength}-{Global.EmailMaxLength}";
                error.En += additional; error.Bg += additional;
                return new ServiceResponseModel(error, null);
            }

            if (!Global.EmailValidator(email))
                return new ServiceResponseModel(this.GetErrors().InvalidEmail, null);

            return null;
        }

        private ServiceResponseModel ValidNewNames(RegisterRequestModel model)
        {
            if (!string.IsNullOrWhiteSpace(model?.FirstName))
            {
                if (model.FirstName.Length < Global.NameMinLength ||
                    Global.NameMaxLength < model.FirstName.Length)
                    return new ServiceResponseModel(this.GetErrors().InvalidName, null);
            }

            if (!string.IsNullOrWhiteSpace(model?.MiddleName))
            {
                if (model.MiddleName.Length < Global.NameMinLength ||
                    Global.NameMaxLength < model.MiddleName.Length)
                    return new ServiceResponseModel(this.GetErrors().InvalidName, null);
            }

            if (!string.IsNullOrWhiteSpace(model?.LastName))
            {
                if (model.LastName.Length < Global.NameMinLength ||
                    Global.NameMaxLength < model.LastName.Length)
                    return new ServiceResponseModel(this.GetErrors().InvalidName, null);
            }

            return null;
        }
    }
}
