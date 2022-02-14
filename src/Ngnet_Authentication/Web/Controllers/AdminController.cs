using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Services.Email;
using Services.Interfaces;
using ApiModels.Users;

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
            AdminResponseModel response = this.adminService.Profile<AdminResponseModel>(this.Claims.UserId);
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(errors);
            }

            response.RoleName = this.Claims.RoleType.ToString();

            return response;
        }

        [HttpGet(nameof(GetUsers))]
        public ActionResult<AdminResponseModel[]> GetUsers() => this.adminService.GetUsers();

        [HttpGet(nameof(GetRoles))]
        public RoleModel[] GetRoles() => this.adminService.GetRoles();

        [HttpGet(nameof(GetEntries))]
        public EntryModel[] GetEntries() => this.adminService.GetEntries();

        [HttpPost(nameof(ChangeRole))]
        public async Task<ActionResult> ChangeRole(AdminRequestModel model)
        {
            if (!this.PermissionsToUser(model.Id))
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
                model.Id = this.Claims.UserId;
            // Tring to make changes on other user
            else
            {
                if (!this.PermissionsToUser(model.Id))
                {
                    this.errors = this.GetErrors().UserNotFound;
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
                model.Id = this.Claims.UserId;
            // Tring to make changes on other user
            else
            {
                if (!this.PermissionsToUser(model.Id))
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
            if (!this.PermissionsToUser(model.Id))
                return this.Unauthorized(this.GetErrors().NoPermissions);

            this.response = await this.adminService.Delete(model.Id);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(DeleteUserAccount))]
        public async Task<ActionResult> DeleteUserAccount(AdminRequestModel model)
        {
            if (!this.PermissionsToUser(model.Id))
                return this.Unauthorized(this.GetErrors().NoPermissions);

            this.response = await this.adminService.DeleteAccount(model.Id);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
