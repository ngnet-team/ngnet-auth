using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ApiModels.Auth;

using Common.Json.Models;
using Common.Json.Service;
using Common.Enums;
using Services.Email;
using Services.Interfaces;
using Web.Controllers.Base;
using ApiModels.Dtos;

namespace Web.Controllers
{
    public class AuthController : ApiController
    {
        protected IAuthService authService;
        protected IEmailSenderService emailSenderService;
        protected ResponseMessage errors;

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

        protected override RoleType RoleRequired { get; } = RoleType.Auth;

        [HttpPost(nameof(Register))]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            this.response = await this.authService.Register(model);
            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            return this.Ok(this.response.Success);
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel model)
        {
            this.response = await this.authService.Login(model);

            if (this.response.Errors != null)
                return this.BadRequest(this.response.Errors);

            UserDto userDto = (UserDto)this.response.RawData;
            JwtTokenModel tokenModel = new JwtTokenModel(this.AppSettings.ApplicationCalls.FirstOrDefault().Key)
            {
                UserId = userDto.Id,
                Username = userDto.Username,
                RoleName = this.authService.GetUserRoleType(userDto.Id)?.ToString(),
            };
            string token = this.authService.CreateJwtToken(tokenModel);

            return new LoginResponseModel { Token = token, ResponseMessage = this.response.Success };
        }
    }
}
