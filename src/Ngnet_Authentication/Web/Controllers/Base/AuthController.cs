using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Common.Json.Models;
using Common.Json.Service;
using Services.Auth;
using Services.Email;
using System.Threading.Tasks;
using ApiModels.Auth;
using Database.Models;
using Common.Enums;

namespace Web.Controllers.Base
{
    public class AuthController : ApiController
    {
        protected IAuthService authService;
        protected IEmailSenderService emailSenderService;
        protected LanguagesModel errors;

        public AuthController
            (IAuthService authService,
             IEmailSenderService emailSenderService,
             IConfiguration configuration,
             JsonService jsonService)
            : base(jsonService, configuration)
        {
            this.authService = authService;
            this.emailSenderService = emailSenderService;
        }

        protected override RoleTitle RoleRequired { get; } = RoleTitle.Guest;

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            if (this.IsAuthenticated)
                return this.BadRequest();

            this.response = await this.authService.Register(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel model)
        {
            if (this.IsAuthenticated)
                return this.BadRequest();

            this.response = await this.authService.Login(model);

            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            User user = (User)this.response.RawData;
            JwtTokenModel tokenModel = new JwtTokenModel(this.configuration["ApplicationSettings:Secret"]) 
            {
                UserId = user.Id,
                Username = user.Username,
                RoleName = this.authService.GetUserRole(user).ToString(),
            };
            string token = this.authService
                .CreateJwtToken(tokenModel);

            return new LoginResponseModel { Token = token, ResponseMessage = this.response.Success };
        }

        protected async Task<ActionResult> UpdateBase<T>(T model)
        {
            this.response = await this.authService.Update<T>(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }
    }
}
