using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels;
using ApiModels.Auth;
using Common;
using Common.Json.Service;
using Common.Enums;
using Services.Seeding;
using Services.Base;
using Web.Infrastructure.Models;

namespace Web.Controllers.Base
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : ControllerBase
    {
        private string token;

        protected readonly JsonService jsonService;
        protected readonly IConfiguration configuration;
        protected ServiceResponseModel response;

        protected ApiController(JsonService jsonService, IConfiguration configuration)
        {
            this.jsonService = jsonService;
            this.AppSettings = configuration.Get<AppSettings>();
        }

        protected string AppName { get; private set; }

        protected ClaimModel Claims { get; private set; }

        protected AppSettings AppSettings { get; }

        protected bool IsAuthenticated => this.ReadHeaders() != null;

        protected bool IsAuthorized => this.IsAuthenticated && this.GetClaims().RoleType <= this.RoleRequired;

        protected abstract RoleType RoleRequired { get; }

        protected UserSeederModel[] Owners => configuration.GetSection(RoleType.Owner.ToString()).Get<UserSeederModel[]>();

        protected UserSeederModel[] Admins => configuration.GetSection(RoleType.Admin.ToString()).Get<UserSeederModel[]>();

        protected ErrorMessagesModel GetErrors()
        {
            return this.jsonService.Deserialiaze<ErrorMessagesModel>(Paths.ErrorMessages);
        }

        protected SuccessMessagesModel GetSuccessMsg()
        {
            return this.jsonService.Deserialiaze<SuccessMessagesModel>(Paths.SuccessMessages);
        }

        private ClaimModel GetClaims()
        {
            if (this.token == null)
                return new ClaimModel();

            var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(this.token);
            var claims = tokenHandler.Claims;
            bool expired = tokenHandler.ValidTo.Date < DateTime.UtcNow;
            if (expired)
                return new ClaimModel();

            RoleType roleType;
            Enum.TryParse(claims.FirstOrDefault(x => x.Type == "role").Value, out roleType);

            if (this.AppName != null)
                Console.WriteLine("App call: " + this.AppName);

            this.Claims = new ClaimModel(roleType)
            {
                UserId = claims.FirstOrDefault(x => x.Type == "nameid").Value,
                Username = claims.FirstOrDefault(x => x.Type == "unique_name").Value,
            };

            return this.Claims;
        }

        private string ReadHeaders()
        {
            var http = this.HttpContext;
            if (http == null)
                return null;

            string token = http.Request.Headers
              .FirstOrDefault(x => x.Key == "Authorization").Value
              .ToString().Replace("Bearer ", "");

            if (token == "")
                return null;

            if (!this.ValidToken(token))
                return null;

            this.token = token;

            string appKey = http.Request.Headers
              .FirstOrDefault(x => x.Key == "AppKey").Value
              .ToString();

            ApplicationCall appCall = this.AppSettings.ApplicationCalls.FirstOrDefault(x => x.Key == appKey);
            if (appCall != null)
                this.AppName = appCall.Name;

            return this.token;
        }

        private bool ValidToken(string token)
        {
            return new JwtSecurityTokenHandler().CanReadToken(token);
        }
    }
}