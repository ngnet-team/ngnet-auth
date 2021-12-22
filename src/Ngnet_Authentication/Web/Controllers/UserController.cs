using Common.Json.Service;
using Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiModels.Users;
using Database.Models;
using System;
using Common;
using Services.Users;
using Microsoft.Extensions.Configuration;
using Services.Email;
using Common.Enums;

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
            

        [HttpGet]
        [Route(nameof(Logout))]
        public async Task<ActionResult> Logout()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            this.response = await this.authService.Logout(this.GetClaims().UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost]
        [Route(nameof(Update))]
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


        [HttpGet]
        [Route(nameof(DeleteAccount))]
        public async Task<ActionResult> DeleteAccount()
        {
            if (!this.IsAuthenticated || this.SeededOwner())
                return this.Unauthorized();

            this.response = await this.userService.DeleteAccount(this.GetClaims().UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost]
        [Route(nameof(ChangeEmail))]
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

            bool valid = this.authService.ValidEmail(model, user);
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

        [HttpPost]
        [Route(nameof(ChangePassword))]
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

            bool valid = this.authService.ValidPassword(model, user);
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

        [HttpPost]
        [Route(nameof(ResetPassword))]
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

        protected User GetUser(string userId = null)
        {
            return this.authService.GetUserById(userId) ??
                   this.authService.GetUserById(this.GetClaims().UserId);
        }
    }
}
