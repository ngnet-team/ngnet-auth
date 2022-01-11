using ApiModels;
using Common;
using Common.Json.Service;
using Database;
using Moq;
using Services.Base;

namespace Tests.Services
{
    public class BaseServiceTests
    {
        protected readonly Mock<NgnetAuthDbContext> fakeDatabase;
        protected readonly JsonService jsonService;
        protected ServiceResponseModel response;

        public BaseServiceTests()
        {
            this.jsonService = new JsonService();
        }

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
