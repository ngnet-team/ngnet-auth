using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Dtos;
using ApiModels.Users;
using Common.Enums;
using Common.Json.Service;
using Services.Email;
using Services.Interfaces;
using Services.Seeding.Models;

namespace Web.Controllers
{
    public class UserController : AuthController
    {
        private readonly IUserService userService;

        public UserController
            (IUserService userService,
             IEmailSenderService emailSenderService,
             IConfiguration configuration,
             JsonService jsonService)
            : base(userService, emailSenderService, configuration, jsonService)
        {
            this.userService = userService;
        }

        protected override RoleType RoleRequired { get; } = RoleType.User;

        [HttpGet(nameof(Profile))]
        public virtual ActionResult<object> Profile()
        {
            UserResponseModel response = this.userService.Profile<UserResponseModel>(this.Claims.UserId);
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(errors);
            }

            return response;
        }

        [HttpGet(nameof(Logout))]
        public async Task<ActionResult> Logout()
        {
            this.response = await this.userService.Logout(this.Claims.UserId, this.Claims.Username);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(Update))]
        public virtual async Task<ActionResult> Update(UpdateRequestModel model)
        {
            model.Id = this.Claims?.UserId;

            this.response = await this.userService.Update(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(Change))]
        public virtual async Task<ActionResult> Change(ChangeRequestModel model)
        {
            model.Id = this.Claims?.UserId;

            this.response = await this.userService.Change(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpGet(nameof(Users))]
        public virtual ActionResult<object> Users()
        {
            UserResponseModel[] users = this.userService.GetUsers<UserResponseModel>();

            return this.Ok(users);
        }

        [HttpGet(nameof(Delete))] // Marked as deleted ONLY!
        public async Task<ActionResult> Delete()
        {
            this.response = await this.userService.Delete(this.Claims.UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpGet(nameof(DeleteAccount))] // PERMANENT deletion!
        public async Task<ActionResult> DeleteAccount()
        {
            this.response = await this.userService.DeleteAccount(this.Claims.UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpGet(nameof(ResetPassword))]
        public async Task<ActionResult> ResetPassword()
        {
            this.response = await userService.ResetPassword(this.Claims.UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response); //TODO: Currently sending the new passowrd as a response but should be changed via Email only
        }

        [HttpGet(nameof(UsersCount))]
        public int UsersCount() => this.userService.UsersCount;

        // ---------------------- Protected ---------------------- 

        protected UserDto GetUser(string userId = null)
        {
            return this.userService.GetUserDtoById(userId) ??
                   this.userService.GetUserDtoById(this.Claims?.UserId);
        }

        protected bool Rights(string userId)
        {
            // No existing user
            if (userId == null)
                return false;
            // Personal permission is always possible
            if (this.Claims?.UserId == userId)
                return true;

            RoleType? currUserRole = this.Claims?.RoleType;
            RoleType? userRole = this.userService.GetUserRoleType(userId);
            // No existing role
            if (currUserRole == null || userRole == null)
                return false;

            return currUserRole < userRole;
        }

        protected bool IsSeeded(RoleType roleType, string userId = null)
        {
            string username = userId == null
                ? this.Claims?.Username // Current user
                : this.GetUser(userId)?.Username; // Other user from db

            if (username == null)
                return false;

            UserSeederModel[] models =
                  RoleType.Owner == roleType ? this.AppSettings?.Seeding?.Owners
                : RoleType.Admin == roleType ? this.AppSettings?.Seeding?.Admins
                : RoleType.Member == roleType ? this.AppSettings?.Seeding?.Members
                : RoleType.User == roleType ? this.AppSettings?.Seeding?.Users
                : null;

            if (models != null && models.Any(x => x.Username == username))
                return true;

            return false;
        }
    }
}
