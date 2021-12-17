using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Common.Json.Service;
using Database.Models;
using Services.Email;
using System.Threading.Tasks;
using ApiModels.Admins;
using Services.Admins;
using Common.Enums;

namespace Web.Controllers
{
    [Authorize(/*Roles = "Admin"*/)]
    public class AdminController : UserController
    {
        private readonly IAdminService adminService;

        public AdminController
            (IAdminService adminService,
             IConfiguration configuration,
             JsonService jsonService,
             IEmailSenderService emailSenderService)
            : base(null, configuration, jsonService, emailSenderService)
        {
            this.adminService = adminService;
        }

        [HttpGet]
        [Route(nameof(GetUsers))]
        public ActionResult<AdminResponseModel[]> GetUsers()
        {
            AdminResponseModel[] users = this.adminService.GetUsers();
            if (users.Length == 0)
            {
                this.errors = this.GetErrors().UsersNotFound;
                return this.BadRequest(this.errors);
            }

            return users;
        }

        [HttpPost]
        [Route(nameof(ChangeRole))]
        public async Task<ActionResult> ChangeRole(AdminRequestModel model)
        {
            model.Id = this.GetUser(model.Id).Id;
            CRUD response = await this.adminService.ChangeRole(model);
            if (response.Equals(CRUD.NoPermissions))
            {
                this.errors = this.GetErrors().NoPermissions;
                return this.BadRequest(this.errors);
            }
            else if (response.Equals(CRUD.NotFound))
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.BadRequest(this.errors);
            }

            return this.Ok(this.GetSuccessMsg().UserUpdated);
        }

        [Authorize]
        [HttpPost]
        [Route(nameof(Update))]
        public async Task<ActionResult> Update(AdminRequestModel model)
        {
            model.Id = this.GetUser(model.Id).Id;
            if (model.Id == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return await this.UpdateBase<AdminRequestModel>(model);
        }

        [Authorize]
        [HttpPost]
        [Route(nameof(ChangeEmail))]
        public async Task<ActionResult> ChangeEmail(AdminChangeModel model)
        {
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

        [Authorize]
        [HttpPost]
        [Route(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(AdminChangeModel model)
        {
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
