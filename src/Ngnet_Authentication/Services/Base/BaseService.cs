using System.Text;

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
            if (string.IsNullOrEmpty(input))
                return null;

            StringBuilder output = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (i == 0)
                    output.Append(char.ToUpper(input[i]));
                else
                    output.Append(char.ToLower(input[i]));
            }

            return output.ToString();
        }
    }
}
