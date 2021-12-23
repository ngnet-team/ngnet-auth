using Common;
using ApiModels;
using Microsoft.AspNetCore.Mvc;
using Common.Json.Service;
using Microsoft.Extensions.Configuration;
using Services.Seeding;
using Services;
using System.Linq;
using ApiModels.Auth;
using System.IdentityModel.Tokens.Jwt;
using Common.Enums;
using System;
using Services.Base;

namespace Web.Controllers.Base
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : ControllerBase
    {
        protected readonly JsonService jsonService;
        protected readonly IConfiguration configuration;
        protected ServiceResponseModel response;

        protected ApiController(JsonService jsonService, IConfiguration configuration)
        {
            this.jsonService = jsonService;
            this.configuration = configuration;
        }

        protected bool IsAuthenticated => this.JwtToken() != null;

        protected bool IsAuthorized => this.GetClaims().RoleTitle <= this.RoleRequired;

        protected abstract RoleTitle RoleRequired { get; }

        protected ClaimModel GetClaims()
        {
            string token = this.JwtToken();
            if (token == null)
                return new ClaimModel();

            var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;

            RoleTitle roleTitle;
            Enum.TryParse<RoleTitle>(claims.FirstOrDefault(x => x.Type == "role").Value, out roleTitle);

            return new ClaimModel(roleTitle)
            {
                UserId = claims.FirstOrDefault(x => x.Type == "nameid").Value,
                Username = claims.FirstOrDefault(x => x.Type == "unique_name").Value,
            };
        }

        protected UserSeederModel[] Owners => configuration.GetSection("Owners").Get<UserSeederModel[]>();

        protected UserSeederModel[] Admins => configuration.GetSection("Admins").Get<UserSeederModel[]>();

        protected ErrorMessagesModel GetErrors()
        {
            return this.jsonService.Deserialiaze<ErrorMessagesModel>(Paths.ErrorMessages);
        }

        protected SuccessMessagesModel GetSuccessMsg()
        {
            return this.jsonService.Deserialiaze<SuccessMessagesModel>(Paths.SuccessMessages);
        }

        private string JwtToken()
        {
            var http = this.HttpContext;
            if (http == null)
                return null;

            string token = http.Request.Headers
              .FirstOrDefault(x => x.Key == "Authorization").Value
              .ToString().Replace("Bearer ", "");

            if (token == "")
                return null;

            return token;
        }
    }
}