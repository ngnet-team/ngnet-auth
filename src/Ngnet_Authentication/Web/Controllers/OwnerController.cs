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

        protected override RoleType RoleRequired { get; } = RoleType.Owner;

        [HttpGet(nameof(Profile))]
        public override ActionResult<object> Profile()
        {
            OwnerResponseModel response = this.ownerService.Profile<OwnerResponseModel>(this.Claims.UserId);
            response.RoleName = this.Claims.RoleType.ToString();

            return response;
        }

        
        [HttpPost(nameof(SetMaxRoles))]
        public async Task<ActionResult> SetMaxRoles(RoleModel[] models)
        {
            this.response = await this.ownerService.SetMaxRoles(models);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
