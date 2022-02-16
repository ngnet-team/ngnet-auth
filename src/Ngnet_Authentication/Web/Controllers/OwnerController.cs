using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Owners;
using Common.Enums;
using Common.Json.Service;
using Services.Email;
using Services.Interfaces;
using ApiModels.Users;

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
            OwnerResponseModel response = (OwnerResponseModel)this.ownerService.GetAccounts<OwnerResponseModel>(this.Claims?.UserId);
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(errors);
            }

            response.RoleName = this.Claims.RoleType.ToString();

            UserOptionalModel complicated = this.ownerService.IncludeComplicated(this.Claims?.UserId);

            response.Address = complicated.Address;
            response.Contact = complicated.Contact;

            return response;
        }

        
        [HttpPost(nameof(SetMaxRoles))]
        public async Task<ActionResult> SetMaxRoles(RoleModel model)
        {
            this.response = await this.ownerService.SetMaxRoles(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
