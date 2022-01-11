using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Services.Email;
using Services.Interfaces;
using ApiModels.Users;
using ApiModels.Dtos;

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
            if (!this.IsAuthorized)
                return this.Unauthorized();
            
            AdminResponseModel response = this.adminService.Profile<AdminResponseModel>(this.Claims.UserId);
            response.RoleName = this.Claims.RoleType.ToString();

            return response;
        }

        [HttpGet(nameof(GetUsers))]
        public ActionResult<AdminResponseModel[]> GetUsers()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            AdminResponseModel[] users = this.adminService.GetUsers();
            if (users.Length == 0)
            {
                this.errors = this.GetErrors().UsersNotFound;
                return this.BadRequest(this.errors);
            }

            return users;
        }

        [HttpGet(nameof(GetRoles))]
        public ActionResult<RoleResponseModel[]> GetRoles()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            RoleResponseModel[] roles = this.adminService.GetRoles();
            if (roles.Length == 0)
            {
                this.errors = this.GetErrors().InvalidRole;
                return this.BadRequest(this.errors);
            }

            return roles;
        }

        [HttpPost(nameof(ChangeRole))]
        public async Task<ActionResult> ChangeRole(AdminRequestModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();
            //Set current logged user id if the model id is null
            if (model.Id == null)
            {
                if (SeededOwner())
                    return this.BadRequest(this.GetErrors().NoPermissions);

                model.Id = this.Claims.UserId;
            }

            this.response = await this.adminService.ChangeRole(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
        
        [HttpPost(nameof(Update))]
        public override async Task<ActionResult> Update(UserRequestModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            if (model.Id == null)
                model.Id = this.Claims.UserId;

            this.response = await this.adminService.Update(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(Change))]
        public override async Task<ActionResult> Change(UserChangeModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            UserDto userDto = this.GetUser(model.Id);
            // Tring to make changes on other user
            if (!this.HasPermissionsToUser(userDto))
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            this.response = this.adminService.Change(model, userDto);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return await this.Update((UserRequestModel)this.response.RawData);
        }

        [HttpPost(nameof(DeleteUser))]
        public async Task<ActionResult> DeleteUser(AdminRequestModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            UserDto userDto = this.adminService.GetDeletableUser(model.Id);
            if (!this.HasPermissionsToUser(userDto))
                return this.Unauthorized(this.GetErrors().NoPermissions);

            this.response = await this.adminService.Delete(userDto.Id);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(DeleteUserAccount))]
        public async Task<ActionResult> DeleteUserAccount(AdminRequestModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            UserDto userDto = this.adminService.GetDeletableUser(model.Id);
            if (!this.HasPermissionsToUser(userDto))
                return this.Unauthorized(this.GetErrors().NoPermissions);

            this.response = await this.adminService.DeleteAccount(userDto.Id);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
