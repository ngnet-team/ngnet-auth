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

        protected bool IsAuthorized => this.GetClaims().RoleType <= this.RoleRequired;

        protected abstract RoleType RoleRequired { get; }

        protected ClaimModel GetClaims()
        {
            string token = this.JwtToken();
            if (token == null)
                return new ClaimModel();

            var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = tokenHandler.Claims;
            bool expired = tokenHandler.ValidTo.Date < DateTime.UtcNow;
            if (expired)
                return new ClaimModel();

            RoleType roleType;
            Enum.TryParse(claims.FirstOrDefault(x => x.Type == "role").Value, out roleType);

            return new ClaimModel(roleType)
            {
                UserId = claims.FirstOrDefault(x => x.Type == "nameid").Value,
                Username = claims.FirstOrDefault(x => x.Type == "unique_name").Value,
            };
        }

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