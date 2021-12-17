using Microsoft.Extensions.Configuration;
using Common.Json.Service;
using Services.Auth;
using Services.Email;
using Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Infrastructure;
using ApiModels.Users;
using ApiModels.Auth;
using Database.Models;
using System;
using Common;

namespace Web.Controllers
{
    public class UserController : AuthController
    {
        public UserController
            (IAuthService userService,
             IConfiguration configuration,
             JsonService jsonService,
             IEmailSenderService emailSenderService)
            : base(userService, configuration, jsonService, emailSenderService)
        {
        }

        [Authorize]
        [HttpPost]
        [Route(nameof(Update))]
        public async Task<ActionResult> Update(UserRequestModel model)
        {
            model.Id = this.User.GetId();
            if (model.Id == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return await this.UpdateBase<UserRequestModel>(model);
        }

        [Authorize]
        [HttpPost]
        [Route(nameof(ChangeEmail))]
        public async Task<ActionResult> ChangeEmail(UserChangeModel model)
        {
            User user = this.GetUser(this.User.GetId());
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

        [Authorize]
        [HttpPost]
        [Route(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(UserChangeModel model)
        {
            User user = this.GetUser(this.User.GetId());
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

        [Authorize]
        [HttpPost]
        [Route(nameof(ResetPassword))]
        public async Task<ActionResult> ResetPassword()
        {
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
                Msg = this.GetSuccessMsg().UserUpdated
            });
        }
    }
}
