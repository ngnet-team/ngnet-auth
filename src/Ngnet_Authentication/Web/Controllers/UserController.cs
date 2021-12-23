﻿using System;
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

        [HttpPost(nameof(Change))]
        public async Task<ActionResult> Change(UserChangeModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            User user = this.GetUser();
            this.response = this.userService.Change(model, user);
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
            RoleType currUserRole = this.GetClaims().RoleType;
            //Higher than the wanted user
            return currUserRole < userRole.Type;
        }
    }
}
