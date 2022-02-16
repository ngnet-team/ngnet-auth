using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;

using ApiModels.Auth;
using Common;
using Common.Enums;
using Web.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Common.Json.Models;

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
            if (RoleType.Auth.Equals(roleInvoked))
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
            if (String.IsNullOrEmpty(this.token))
                return;
            //TODO: Adding token in every request can't expire it
            context.HttpContext.Response.Cookies.Append(constants.CookieKey, this.token,
                new CookieOptions { Expires = DateTime.Now.AddDays(constants.TokenExpires) });
        }

        private bool NullsOnly(object instance)
        {
            return Global.NullableObject(instance);
        }

        private ClaimModel GetClaims(ActionExecutingContext context)
        {
            //No token
            if (string.IsNullOrEmpty(this.token))
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.MissingToken);
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(this.token);
            var claims = tokenHandler.Claims;
            //Check secret key
            string tokenSecretKey = claims.FirstOrDefault(x => x.Type == "aud")?.Value;
            if (this.appSettings?.SecretKey != tokenSecretKey)
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.InvalidSecretKey);
                return null;
            }
            //Check if token is expired
            bool expired = tokenHandler.ValidTo.Date < DateTime.UtcNow;
            if (expired)
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.TokenExpired);
                return null;
            }
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
                UserId = claims.FirstOrDefault(x => x.Type == "nameid")?.Value,
                Username = claims.FirstOrDefault(x => x.Type == "unique_name")?.Value,
            };
            //Check all claims are assigned
            if (Global.AnyNullObject(claimModel))
            {
                context.Result = new UnauthorizedObjectResult(serverErrors.InvalidToken);
                return null;
            }

            return claimModel;
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
