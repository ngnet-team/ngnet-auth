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

        protected override RoleTitle RoleRequired { get; } = RoleTitle.User;

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
        public async Task<ActionResult> Update(UserRequestModel model)
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

        [HttpPost(nameof(ChangeEmail))]
        public async Task<ActionResult> ChangeEmail(UserChangeModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            User user = this.GetUser();
            if (user == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            bool valid = this.userService.ValidEmail(model, user);
            if (!valid)
            {
                this.errors = this.GetErrors().InvalidEmail;
                return this.Unauthorized(this.errors);
            }

            return await this.Update(new UserRequestModel()
            {
                Email = model.New,
            });
        }

        [HttpPost(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(UserChangeModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            User user = this.GetUser();
            if (user == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            bool valid = this.userService.ValidPassword(model, user);
            if (!valid)
            {
                this.errors = this.GetErrors().InvalidEmail;
                return this.Unauthorized(this.errors);
            }

            return await this.Update(new UserRequestModel()
            {
                Password = model.New,
            });
        }

        [HttpPost(nameof(ResetPassword))]
        public async Task<ActionResult> ResetPassword()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            User user = this.GetUser();
            if (user == null)
            {
                this.errors = GetErrors().UserNotFound;
                return this.BadRequest(errors);
            }

            string newPassword = Guid.NewGuid().ToString().Substring(0, Global.ResetPasswordLength);
            var result = await this.Update(new UserRequestModel()
            {
                Password = newPassword,
            });

            return this.Ok(new
            {
                NewPassword = newPassword,
                Msg = this.GetSuccessMsg().Updated
            });
        }

        // ---------------------- Abstract ---------------------- 

        protected User GetUser(string userId = null)
        {
            return this.userService.GetUserById(userId) ??
                   this.userService.GetUserById(this.GetClaims().UserId);
        }

        protected bool HasPermissionsToUser(User user)
        {
            if (user == null)
                return false;

            Role userRole = this.userService.GetUserRole(user);
            RoleTitle currUserRole = this.GetClaims().RoleTitle;
            //Higher than the wanted user
            return currUserRole < userRole.Title;
        }
    }
}
