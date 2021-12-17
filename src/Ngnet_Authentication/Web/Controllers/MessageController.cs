using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ApiModels;
using Common.Json.Service;
namespace Web.Controllers.Base
{
    public class MessageController : ApiController
    {

        public MessageController
            (JsonService jsonService,
            IConfiguration configuration)
            : base(jsonService, configuration)
        {
        }

    }
}
