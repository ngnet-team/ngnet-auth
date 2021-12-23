using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Owners;
using Common.Enums;
using Common.Json.Service;
using Services.Email;
using Services.Interfaces;

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

        [HttpGet(nameof(Profile))]
        public override ActionResult<object> Profile()
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            OwnerResponseModel response = this.ownerService.Profile<OwnerResponseModel>(this.GetClaims().UserId);
            //Add current user's role
            response.RoleName = this.GetClaims().RoleTitle.ToString();
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return response;
        }

        
        [HttpPost(nameof(SetRoleCounts))]
        public async Task<ActionResult> SetRoleCounts(MaxRoles model)
        {
            if (!this.IsAuthorized)
                return this.Unauthorized();

            this.response = await this.ownerService.SetRoleCounts(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
