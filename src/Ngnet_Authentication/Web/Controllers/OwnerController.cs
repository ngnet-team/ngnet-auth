using ApiModels.Admins;
using Common.Enums;
using Common.Json.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Services.Email;
using Services.Owners;

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
            : base(null, emailSenderService, configuration, jsonService)
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
    }
}
