using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Linq;

using Common;
using Common.Json.Models;
using Web.Infrastructure.Models;

namespace Web.Infrastructure.Filters
{
    public class HttpFilter : IActionFilter
    {
        private static ConstantsModel constants = Global.Constants;
        private static ServerErrorsModel serverErrors = Global.ServerErrors;
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
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private bool NullsOnly(object instance)
        {
            return Global.NullableObject(instance);
        }

        private bool ValidApiKey(ActionExecutingContext context)
        {
            string serverApiKey = this.appSettings?.ApiKey;

            string clientApiKey = context.HttpContext.Request.Headers
                .FirstOrDefault(x => Global.AllCapital(x.Key) == constants.ApiKeyHeaderKey).Value
                .ToString();

            return serverApiKey == clientApiKey;
        }
    }
}
