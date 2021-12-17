using Common;
using ApiModels;
using Microsoft.AspNetCore.Mvc;
using Common.Json.Service;
using Microsoft.Extensions.Configuration;
using Services.Seeding;

namespace Web.Controllers.Base
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : ControllerBase
    {
        protected readonly JsonService jsonService;
        protected readonly IConfiguration configuration;

        protected ApiController(JsonService jsonService, IConfiguration configuration)
        {
            this.jsonService = jsonService;
            this.configuration = configuration;
        }

        protected UserSeederModel Admin => configuration.GetSection("Admin").Get<UserSeederModel>();

        protected ErrorMessagesModel GetErrors()
        {
            return this.jsonService.Deserialiaze<ErrorMessagesModel>(Paths.ErrorMessages);
        }

        protected SuccessMessagesModel GetSuccessMsg()
        {
            return this.jsonService.Deserialiaze<SuccessMessagesModel>(Paths.SuccessMessages);
        }
    }
}