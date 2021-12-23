using Microsoft.AspNetCore.Mvc;
using Common.Json.Service;
using Database.Models;
using System.Threading.Tasks;
using ApiModels.Admins;
using Microsoft.Extensions.Configuration;
using Services.Email;
using Common.Enums;
using Services.Interfaces;

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

        protected override RoleTitle RoleRequired { get; } = RoleTitle.Admin;

        [HttpGet]
        [Route(nameof(Profile))]
        public override ActionResult<object> Profile()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            AdminResponseModel response = this.adminService.Profile<AdminResponseModel>(this.GetClaims().UserId);
            //Add current user's role
            response.RoleName = this.GetClaims().RoleTitle.ToString();
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return response;
        }

        [HttpGet]
        [Route(nameof(GetUsers))]
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

        [HttpGet]
        [Route(nameof(GetRoles))]
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

        [HttpPost]
        [Route(nameof(ChangeRole))]
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

        [HttpPost]
        [Route(nameof(Update))]
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

        [HttpPost]
        [Route(nameof(ChangeEmail))]
        public async Task<ActionResult> ChangeEmail(AdminChangeModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            User user = this.GetUser(model.Id);
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

            return await this.Update(new AdminRequestModel()
            {
                Email = model.New,
            });
        }

        [HttpPost]
        [Route(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(AdminChangeModel model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            User user = this.GetUser(model.Id);
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

            return await this.Update(new AdminRequestModel()
            {
                Password = model.New,
            });
        }
    }
}
