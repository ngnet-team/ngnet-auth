using ApiModels.Admins;
using ApiModels.Owners;
using Common.Enums;
using Common.Json.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Services.Email;
using Services.Interfaces;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class OwnerController : AdminController
    {
        private readonly IOwnerService ownerService;

        public OwnerController
            (IOwnerService ownerService,
             IEmailSenderService emailSenderService,
             IConfiguration configuration,
             JsonService jsonService)
            : base(ownerService, emailSenderService, configuration, jsonService)
        {
            this.ownerService = ownerService;
        }

        protected override RoleTitle RoleRequired { get; } = RoleTitle.Owner;

        [HttpGet]
        [Route(nameof(Profile))]
        public override ActionResult<object> Profile()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            AdminResponseModel response = this.ownerService.Profile<AdminResponseModel>(this.GetClaims().UserId);
            //Add current user's role
            response.RoleName = this.GetClaims().RoleTitle.ToString();
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return response;
        }

        
        [HttpPost]
        [Route(nameof(SetRoleMembers))]
        public async Task<ActionResult> SetRoleMembers(MaxRoles model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            this.response = await this.ownerService.SetRoleMembers(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
