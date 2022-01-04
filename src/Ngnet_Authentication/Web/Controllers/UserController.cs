using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Users;
using Common;
using Common.Enums;
using Common.Json.Service;
using Database.Models;
using Services.Email;
using Services.Interfaces;
using ApiModels.Dtos;

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
            if (!this.IsAuthorized)
                return this.Unauthorized();

            UserResponseModel response = this.userService.Profile<UserResponseModel>(this.GetClaims().UserId);
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return response;
        }

        [HttpGet(nameof(Logout))]
        public async Task<ActionResult> Logout()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            this.response = await this.userService.Logout(this.GetClaims().UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(Update))]
        public virtual async Task<ActionResult> Update(UserRequestModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            model.Id = this.GetClaims().UserId;
            if (model.Id == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return await this.UpdateBase<UserRequestModel>(model);
        }

        [HttpPost(nameof(Change))]
        public async Task<ActionResult> Change(UserChangeModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            UserDto userDto = this.GetUser();
            this.response = this.userService.Change(model, userDto);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return await this.Update((UserRequestModel)this.response.RawData);
        }

        [HttpGet(nameof(DeleteAccount))]
        public async Task<ActionResult> DeleteAccount()
        {
            if (!this.IsAuthenticated || this.SeededOwner())
                return this.Unauthorized();

            this.response = await this.userService.DeleteAccount(this.GetClaims().UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(ResetPassword))]
        public async Task<ActionResult> ResetPassword()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            this.response = await userService.ResetPassword(this.GetClaims().UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        // ---------------------- Abstract ---------------------- 

        protected UserDto GetUser(string userId = null)
        {
            return this.userService.GetUserById(userId) ??
                   this.userService.GetUserById(this.GetClaims().UserId);
        }

        protected bool HasPermissionsToUser(UserDto userDto)
        {
            if (userDto == null)
                return false;

            Role userRole = this.userService.GetUserRole(userDto);
            RoleType currUserRole = this.GetClaims().RoleType;
            //Higher than the wanted user
            return currUserRole < userRole.Type;
        }
    }
}
