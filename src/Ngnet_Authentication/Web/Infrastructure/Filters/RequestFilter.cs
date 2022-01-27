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

namespace Web.Infrastructure.Filters
{
    public class RequestFilter : IActionFilter
    {
        private string token;
        private readonly AppSettings appSettings;

        public RequestFilter(IConfiguration configuration)
        {
            this.appSettings = configuration.Get<AppSettings>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Request Method
            string requestType = context.HttpContext.Request.Method;
            if (requestType.Equals("POST"))
            {
                var body = context.ActionArguments.FirstOrDefault().Value;
                if (this.NullsOnly(body))
                {
                    context.Result = new BadRequestObjectResult("No body");
                    return;
                }
            }
            //Get JWT token
            this.token = context.HttpContext.Request.Headers
                .FirstOrDefault(x => x.Key == "Authorization").Value
                .ToString().Replace("Bearer", "").Trim();
            //Get invoked controller name/role name
            ActionPath action = this.GetPath(context.ActionDescriptor.DisplayName);
            if (action == null)
            {
                context.Result = new BadRequestObjectResult("No action path");
                return;
            }
            RoleType roleInvoked;
            bool validRole = Enum.TryParse<RoleType>(action.Controller, true, out roleInvoked);
            if (!validRole)
            {
                context.Result = new UnauthorizedObjectResult("Invalid role in url");
                return;
            }
            //Guest
            if (RoleType.Auth.Equals(roleInvoked))
            {
                //Token exists
                if (!string.IsNullOrEmpty(this.token))
                {
                    context.Result = new BadRequestObjectResult("Logout first");
                    return;
                }
            }
            //Logged user in role
            else
            {
                if (action.Method == "Login" || action.Method == "Register")
                {
                    context.Result = new BadRequestObjectResult("Logout first");
                    return;
                }

                ClaimModel claims = this.GetClaims(context);
                if (claims == null)
                    return;
                //token role has no permissions to invoked controller
                if (roleInvoked < claims.RoleType)
                {
                    context.Result = new UnauthorizedObjectResult("Invalid role in url");
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
            //No token
            if (string.IsNullOrEmpty(this.token))
            {
                context.Result = new UnauthorizedObjectResult("No jwt token");
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(this.token);
            var claims = tokenHandler.Claims;
            //Check secret key
            string secretKey = claims.FirstOrDefault(x => x.Type == "aud")?.Value;
            string appName = this.ReadSecretKey(secretKey);
            if (string.IsNullOrEmpty(appName))
            {
                context.Result = new UnauthorizedObjectResult("No secret key");
                return null;
            }
            //Check if token is expired
            bool expired = tokenHandler.ValidTo.Date < DateTime.UtcNow;
            if (expired)
            {
                context.Result = new UnauthorizedObjectResult("Token has been expired");
                return null;
            }
            //Get role from token
            RoleType roleType;
            bool validRole = Enum.TryParse(claims.FirstOrDefault(x => x.Type == "role")?.Value, out roleType);
            if (!validRole)
            {
                context.Result = new UnauthorizedObjectResult("Invalid role");
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
                context.Result = new UnauthorizedObjectResult("Missing token parts");
                return null;
            }

            return claimModel;
        }

        private string ReadSecretKey(string secretKey)
        {
            ApplicationCall appCall = this.appSettings.ApplicationCalls.FirstOrDefault(x => x.Key == secretKey);
            if (appCall == null)
                return null;

            return appCall?.Name;
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
