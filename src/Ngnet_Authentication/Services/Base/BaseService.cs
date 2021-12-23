using ApiModels;
using Common;
using Common.Json.Service;
using Database;

namespace Services.Base
{
    public abstract class BaseService
    {
        protected readonly NgnetAuthDbContext database;
        protected readonly JsonService jsonService;
        protected ServiceResponseModel response;

        protected BaseService(NgnetAuthDbContext database, JsonService jsonService)
        {
            this.database = database;
            this.jsonService = jsonService;
        }

        protected ErrorMessagesModel GetErrors()
        {
            return this.jsonService.Deserialiaze<ErrorMessagesModel>(Paths.ErrorMessages);
        }

        protected SuccessMessagesModel GetSuccessMsg()
        {
            return this.jsonService.Deserialiaze<SuccessMessagesModel>(Paths.SuccessMessages);
        }

        protected string Capitalize(string input)
        {
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
