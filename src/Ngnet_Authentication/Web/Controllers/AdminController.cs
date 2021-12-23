using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Database.Models;
using Services.Email;
using Services.Interfaces;
using ApiModels.Users;

namespace Web.Controllers
{
    public class AdminController : UserController
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
            
            AdminResponseModel response = this.adminService.Profile<AdminResponseModel>(this.GetClaims().UserId);
            response.RoleName = this.GetClaims().RoleType.ToString();

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

                model.Id = this.GetClaims().UserId;
            }

            this.response = await this.adminService.ChangeRole(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
        //TODO: multiple matches in endpoint update and change
        [HttpPost(nameof(Update))]
        public async Task<ActionResult> Update(AdminRequestModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            model.Id = this.GetClaims().UserId;
            if (model.Id == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return await this.UpdateBase<AdminRequestModel>(model);
        }

        [HttpPost(nameof(Change))]
        public async Task<ActionResult> Change(AdminChangeModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            User user = this.GetUser(model.Id);
            //Tring to make change on other user
            if (model.Id != null)
            {
                if (!this.HasPermissionsToUser(user))
                {
                    this.errors = this.GetErrors().UserNotFound;
                    return this.Unauthorized(this.errors);
                }
            }

            this.response = this.adminService.Change(model, user);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return await this.Update((UserRequestModel)this.response.RawData);
        }

        [HttpPost(nameof(DeleteUser))]
        public async Task<ActionResult> DeleteUser(AdminRequestModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            User user = this.adminService.GetDeletableUser(model.Id);
            if (!this.HasPermissionsToUser(user))
                return this.Unauthorized(this.GetErrors().NoPermissions);

            this.response = await this.adminService.DeleteUser(user);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
