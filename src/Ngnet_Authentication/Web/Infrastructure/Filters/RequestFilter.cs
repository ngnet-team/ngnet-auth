using Common;
using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Web.Infrastructure.Filters
{
    public class RequestFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Request Method
            string requestType = context.HttpContext.Request.Method;
            RequestMethodType reqMethod;
            Enum.TryParse<RequestMethodType>(requestType, true, out reqMethod);
            if (reqMethod.Equals(RequestMethodType.GET))
            {
                
            }
            else if (reqMethod.Equals(RequestMethodType.POST))
            {
                var model = context.ActionArguments.FirstOrDefault().Value;
                if (this.NullsOnly(model))
                {
                    context.Result = new BadRequestObjectResult("No body");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var token = context.HttpContext.Request.Headers
                .FirstOrDefault(x => x.Key == "Authorization").Value
                .ToString().Replace("Bearer ", "");

            var controllerName = context.Controller.GetType().Name.Replace("Controller", "");
            RoleType roleType;
            Enum.TryParse<RoleType>(controllerName, true, out roleType);
            //Guest
            if (RoleType.Guest.Equals(roleType))
            {

            }
            //LoggedUser
            else
            {

            }
        }

        private bool NullsOnly(object instance)
        {
            return Global.NullableObject(instance);
        }
    }
}
