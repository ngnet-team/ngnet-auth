using System.Text;

using ApiModels.Common;
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

        protected ClientErrorModel GetErrors()
        {
            return this.jsonService.Deserialiaze<ClientErrorModel>(Paths.ClientErrors);
        }

        protected ClientSuccessModel GetSuccessMsg()
        {
            return this.jsonService.Deserialiaze<ClientSuccessModel>(Paths.ClientSuccess);
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
