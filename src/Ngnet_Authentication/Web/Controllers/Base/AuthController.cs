using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Common.Json.Models;
using Common.Json.Service;
using Database.Models;
using Services.Auth;
using Services.Email;
using Web.Infrastructure;
using System.Threading.Tasks;
using Common.Enums;
using ApiModels.Users;
using ApiModels.Auth;

namespace Web.Controllers.Base
{
    public abstract class AuthController : ApiController
    {
        protected IAuthService authService;
        protected IEmailSenderService emailSenderService;
        protected LanguagesModel errors;

        protected AuthController
            (IAuthService authService,
             IConfiguration configuration,
             JsonService jsonService,
             IEmailSenderService emailSenderService)
            : base(jsonService, configuration)
        {
            this.authService = authService;
            this.emailSenderService = emailSenderService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            CRUD response = await this.authService.Register(model);
            if (CRUD.Invalid.Equals(response))
            {
                this.errors = this.GetErrors().NotEqualPasswords;
                //this.errors = this.GetErrors().ExistingUserName;
                return this.BadRequest(this.errors);
            }

            return this.Ok(this.GetSuccessMsg().UserRegistered);
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel model)
        {
            CRUD response = await this.authService.Login(model);
            if (CRUD.NotFound.Equals(response))
            {
                this.errors = this.GetErrors().InvalidUsername;
                return this.Unauthorized(this.errors);
            }

            if (CRUD.Invalid.Equals(response))
            {
                this.errors = this.GetErrors().InvalidPasswords;
                return this.Unauthorized(this.errors);
            }

            string token = this.authService
                .CreateJwtToken(this.User.GetId(), model.Username, this.configuration["ApplicationSettings:Secret"]);
            var responseMessage = this.GetSuccessMsg().UserLoggedIn;
            return new LoginResponseModel { Token = token, ResponseMessage = responseMessage };
        }

        [HttpGet]
        [Route(nameof(Logout))]
        public async Task<ActionResult> Logout()
        {
            CRUD response = await this.authService.Logout(this.GetUser().Id);
            if (!CRUD.Created.Equals(response))
            {
                this.errors = this.GetErrors().NoPermissions;
                return this.Unauthorized(this.errors);
            }

            return this.Ok(this.GetSuccessMsg().UserUpdated);
        }

        [Authorize]
        [HttpGet]
        [Route(nameof(Profile))]
        public ActionResult<UserResponseModel> Profile()
        {
            UserResponseModel response = this.authService.Profile(this.User.GetId());
            if (response == null)
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            return response;
        }

        protected async Task<ActionResult> UpdateBase<T>(T model)
        {
            CRUD result = await this.authService.Update<T>(model);
            if (result.Equals(CRUD.NotFound))
            {
                this.errors = this.GetErrors().UserNotFound;
                return this.Unauthorized(this.errors);
            }

            if (result.Equals(CRUD.None))
            {
                return this.Ok();
            }

            return this.Ok(this.GetSuccessMsg().UserUpdated);
        }

        protected User GetUser(string userId = null)
        {
            if (userId == null)
            {
                return this.authService.GetUser(this.User.GetId());
            }

            return this.authService.GetUser(userId);
        }
    }
}
