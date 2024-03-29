﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Users;
using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Services.Email;
using Services.Interfaces;

namespace Web.Controllers
{
    public class AdminController : MemberController
    {
        private readonly IAdminService adminService;

        public AdminController
            (IAdminService adminService,
             IEmailSenderService emailSenderService,
             IConfiguration configuration,
             JsonService jsonService)
            : base(adminService, emailSenderService, configuration, jsonService)
        {
            this.adminService = adminService;
        }

        protected override RoleType RoleRequired { get; } = RoleType.Admin;

        [HttpGet(nameof(Profile))]
        public override ActionResult<object> Profile()
        {
            AdminResponseModel response = (AdminResponseModel)this.adminService.GetAccounts<AdminResponseModel>(this.Claims?.UserId);
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(errors);
            }

            response.RoleName = this.Claims.RoleType.ToString();

            UserOptionalModel complicated = this.adminService.IncludeComplicated(this.Claims?.UserId);

            response.Address = complicated.Address;
            response.Contact = complicated.Contact;

            return response;
        }

        [HttpGet(nameof(Users))]
        public override ActionResult<object> Users()
        {
            AdminResponseModel[] users = (AdminResponseModel[])this.adminService.GetAccounts<AdminResponseModel>(null);

            foreach (var user in users)
            {
                user.RoleName = this.adminService.GetUserRoleType(user.Id)?.ToString();
            }

            return this.Ok(users);
        }

        [HttpGet(nameof(Roles))]
        public RoleModel[] Roles() => this.adminService.GetRoles();

        [HttpGet(nameof(Entries))]
        public ActionResult<EntryModel[]> Entries()
        {
            EntryModel[] entries = this.adminService.GetEntries();

            return this.Ok(entries);
        }

        [HttpGet(nameof(RightsChanges))]
        public ActionResult<RightsChangeModel[]> RightsChanges()
        {
            RightsChangeModel[] rights = this.adminService.GetRightsChanges();

            return this.Ok(rights);
        }

        [HttpPost(nameof(ChangeRole))]
        public async Task<ActionResult> ChangeRole(AdminRequestModel model)
        {
            if (!this.Rights(model.Id))
                return this.BadRequest(this.GetErrors().NoPermissions);

            this.response = await this.adminService.ChangeRole(model, this.Claims.UserId);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(Update))]
        public override async Task<ActionResult> Update(UpdateRequestModel model)
        {
            if (model.Id == null)
                model.Id = this.Claims?.UserId;
            // Tring to make changes on other user
            else
            {
                if (!this.Rights(model.Id))
                {
                    this.errors = this.GetErrors().NoPermissions;
                    return this.Unauthorized(this.errors);
                }
            }

            this.response = await this.adminService.Update(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(Change))]
        public override async Task<ActionResult> Change(ChangeRequestModel model)
        {
            if (model.Id == null)
                model.Id = this.Claims?.UserId;
            // Tring to make changes on other user
            else
            {
                if (!this.Rights(model.Id))
                {
                    this.errors = this.GetErrors().UserNotFound;
                    return this.Unauthorized(this.errors);
                }
            }

            this.response = await this.adminService.Change(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(DeleteUser))]
        public async Task<ActionResult> DeleteUser(AdminRequestModel model)
        {
            if (!this.Rights(model.Id))
                return this.Unauthorized(this.GetErrors().NoPermissions);

            this.response = await this.adminService.Delete(model.Id);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(DeleteUserAccount))]
        public async Task<ActionResult> DeleteUserAccount(AdminRequestModel model)
        {
            if (!this.Rights(model.Id))
                return this.Unauthorized(this.GetErrors().NoPermissions);

            this.response = await this.adminService.DeleteAccount(model.Id);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
