using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System;

using ApiModels.Guest;
using Common;
using Common.Enums;
using Common.Json.Models;
using Web.Infrastructure.Models;

namespace Web.Infrastructure.Filters
{
    public class HttpFilter : IActionFilter
    {
        private static ConstantsModel constants = Global.Constants;
        private static ServerErrorsModel serverErrors = Global.ServerErrors;
        private string token;
        private readonly AppSettings appSettings;

        public HttpFilter(IConfiguration configuration)
        {
            this.appSettings = configuration.Get<AppSettings>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Validate Api Key
            if (!this.ValidApiKey(context))
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.InvalidApiKey);
                return;
            }

            //Request Method
            string requestType = context.HttpContext.Request.Method;
            if (requestType.Equals("POST"))
            {
                var body = context.ActionArguments.FirstOrDefault().Value;
                if (this.NullsOnly(body))
                {
                    context.Result = new BadRequestObjectResult(serverErrors.MissingBody);
                    return;
                }
            }
            //Get JWT token
            this.token = context.HttpContext.Request.Headers
                .FirstOrDefault(x => x.Key == constants.AuthHeaderKey).Value
                .ToString().Replace(constants.AuthHeaderPreValue, "").Trim();
            //Get invoked controller name/role name
            ActionPath action = this.GetPath(context.ActionDescriptor.DisplayName);
            if (action == null)
            {
                context.Result = new BadRequestObjectResult(serverErrors.NotParsedAction);
                return;
            }
            RoleType roleInvoked;
            bool validRole = Enum.TryParse<RoleType>(action.Controller, true, out roleInvoked);
            if (!validRole)
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.IvalidRole);
                return;
            }
            //Guest
            if (RoleType.Guest.Equals(roleInvoked))
            {
                //Token exists
                if (!string.IsNullOrEmpty(this.token))
                {
                    context.Result = new BadRequestObjectResult(serverErrors.LoggoutFirst);
                    return;
                }
            }
            //Logged user in role
            else
            {
                if (action.Method == "Login" || action.Method == "Register")
                {
                    context.Result = new BadRequestObjectResult(serverErrors.LoggoutFirst);
                    return;
                }

                ClaimModel claims = this.GetClaims(context);
                if (claims == null)
                    return;
                //token role has no permissions to invoked controller
                if (roleInvoked < claims.RoleType)
                {
                    context.Result = new UnauthorizedObjectResult(serverErrors.IvalidRole);
                    return;
                }

                context.HttpContext.Items.Add("UserId", claims.UserId);
                context.HttpContext.Items.Add("Username", claims.Username);
                context.HttpContext.Items.Add("RoleType", claims.RoleType);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private bool NullsOnly(object instance)
        {
            return Global.NullableObject(instance);
        }

        private ClaimModel GetClaims(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(this.token))
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.MissingToken);
                return null;
            }

            if (!this.ValidToken(this.token))
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.InvalidToken);
                return null;
            }

            JwtSecurityToken handler = new JwtSecurityTokenHandler().ReadJwtToken(this.token);
            IEnumerable<Claim> claims = handler.Claims;

            //Get role from token
            RoleType roleType;
            bool validRole = Enum.TryParse(claims.FirstOrDefault(x => x.Type == "role")?.Value, out roleType);
            if (!validRole)
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.IvalidRole);
                return null;
            }
            //Assign claims
            ClaimModel claimModel = new ClaimModel(roleType)
            {
                UserId = claims.FirstOrDefault(x => x.Type == "userid")?.Value,
                Username = claims.FirstOrDefault(x => x.Type == "username")?.Value,
            };
            //Check all claims are assigned
            if (Global.AnyNullObject(claimModel))
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.InvalidToken);
                return null;
            }

            return claimModel;
        }

        private bool ValidToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireExpirationTime = true,
                    ValidIssuer = this.appSettings.Issuer,
                    ValidAudience = this.appSettings.SecretKey, //TODO: for audience should be used the client domain 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.appSettings.SecretKey))
                }, out validatedToken);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                //TODO: Log error
                return false;
            }
            return true;
        }

        private bool ValidApiKey(ActionExecutingContext context)
        {
            string serverApiKey = this.appSettings?.ApiKey;

            string clientApiKey = context.HttpContext.Request.Headers
                .FirstOrDefault(x => Global.AllCapital(x.Key) == constants.ApiKeyHeaderKey).Value
                .ToString();

            return serverApiKey == clientApiKey;
        }

        private ActionPath GetPath(string actionFullName)
        {
            if (actionFullName == null)
                return null;

            string[] actionSpliter = actionFullName.Split('.');

            if (actionSpliter.Length != 4)
                return null;

            return new ActionPath()
            {
                Controller = actionSpliter[2].Replace("Controller", ""),
                Method = actionSpliter[3].Split(' ')[0],
            };
        }
    }
}
