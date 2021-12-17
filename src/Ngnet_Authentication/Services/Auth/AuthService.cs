using Microsoft.IdentityModel.Tokens;
using Common.Json.Service;
using Database;
using Database.Models;
using Mapper;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common;
using System.Text.RegularExpressions;
using ApiModels.Users;
using ApiModels.Auth;

namespace Services.Auth
{
    public class AuthService : BaseService, IAuthService
    {
        public AuthService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public async Task<CRUD> Register(RegisterRequestModel model)
        {
            //Equal passwords check
            if (model.Password != model.RepeatPassword)
                return CRUD.Invalid;
            //Username exists
            User user = this.database.Users.FirstOrDefault(x => x.Username == model.Username);
            if (user != null)
                return CRUD.Invalid;
            //TODO: Email validator

            user = MappingFactory.Mapper.Map<User>(model);
            //user = new User
            //{
            //    Email = model.Email,
            //    Username = model.UserName,
            //    FirstName = model.FirstName,
            //    LastName = model.LastName,
            //    CreatedOn = DateTime.UtcNow
            //};

            await this.database.Users.AddAsync(user);
            Role role = this.database.Roles.FirstOrDefault(x => x.Title == model.Role) ??
                        this.database.Roles.FirstOrDefault(x => x.Title == RoleTitle.User);

            await this.database.UserRoles.AddAsync(new UserRole()
            {
                User = user,
                Role = role
            });

            return CRUD.Created;

            //sendgrid is not ready yet
            //EmailSenderModel email = new EmailSenderModel(this.Admin.Email, model.Email)
            //{
            //    Content = this.emailSenderService.GetTemplate(Paths.SuccessfulRegistration)
            //};
            //var response = await this.emailSenderService.EmailConfirmation(email);

            //return this.Ok(this.GetSuccessMsg().UserRegistered);

            //return this.response;
        }

        public async Task<CRUD> Login(LoginRequestModel model)
        {
            //Username does Not exist
            User user = this.database.Users.FirstOrDefault(x => x.Username == model.Username);
            if (user != null)
                return CRUD.NotFound;
            //Invalid password
            if (user.PasswordHash == Hash.CreatePassword(model.Password))
                return CRUD.Invalid;
            //Already deleted user
            if (user.IsDeleted)
                return CRUD.Invalid;

            await this.AddExperience(new UserExperience()
            {
                UserId = user.Id,
                LoggedIn = DateTime.UtcNow
            });

            return CRUD.Created;
        }

        public string CreateJwtToken(string userId, string username, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }

        public async Task<CRUD> Logout(string userId)
        {
            await this.AddExperience(new UserExperience()
            {
                UserId = userId,
                LoggedOut = DateTime.UtcNow
            });

            return CRUD.Created;
        }

        public UserResponseModel Profile(string userId)
        {
            //User Not found
            User user = this.database.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null || user.IsDeleted)
                return null;

            return MappingFactory.Mapper.Map<UserResponseModel>(user);

            //return new UserResponseModel()
            //{
            //    Email = user.Email,
            //    UserName = user.Username,
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    Gender = user.Gender,
            //    //Age = user.Age
            //};
        }

        public async Task<CRUD> Update<T>(T model)
        {
            CRUD response = CRUD.None;

            User mappedModel = MappingFactory.Mapper.Map<User>(model);

            User user = this.database.Users.FirstOrDefault(x => x.Id == mappedModel.Id);
            if (user == null)
            {
                response = CRUD.NotFound;
            }

            user = this.ModifyEntity(mappedModel, user);

            var result = await this.database.SaveChangesAsync();
            if (result > 0)
            {
                response = CRUD.Updated;
            }

            return response;
        }

        public async Task<CRUD> AddExperience(UserExperience exp)
        {
            CRUD response = CRUD.None;

            User user = this.database.Users.FirstOrDefault(x => x.Id == exp.UserId);
            if (user == null)
            {
                return response = CRUD.NotFound;
            }

            user.Experiences.Add(exp);
            var result = await this.database.SaveChangesAsync();
            if (result > 0)
            {
                response = CRUD.Updated;
            }

            return response;
        }

        public ICollection<ExperienceModel> GetExperiences(string UserId)
        {
            return this.database.UserExperiences.Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.Id)
                .To<ExperienceModel>()
                //To avoid too many records in client
                .Take(20)
                .OrderByDescending(x => x.LoggedIn)
                .ThenByDescending(x => x.LoggedOut)
                .ToHashSet();
        }

        public User GetUser(string userId)
        {
            return this.database.Users.FirstOrDefault(x => x.Id == userId);
        }

        public Role GetRoleByString(string roleName)
        {
            RoleTitle roleTitle;
            bool valid = Enum.TryParse<RoleTitle>(roleName, out roleTitle);
            if (!valid)
                return null;

            return this.database.Roles.FirstOrDefault(x => x.Title == roleTitle);
        }

        public Role GetRoleByEnum(RoleTitle roleTitle)
        {
            return this.database.Roles.FirstOrDefault(x => x.Title == roleTitle);
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

        // ------------------- Private ------------------- 
        private User ModifyEntity(User mappedModel, User user)
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

        private bool ValidChange(UserChangeModel model, string userId)
        {
            //Should be equal
            if (model.New != model.Old)
                return false;
            //User Not Exists
            User user = this.GetUser(userId);
            if (user == null || user.IsDeleted)
                return false;

            return true;
        }

        private bool EmailValidator(string emailAddress)
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
    }
}
