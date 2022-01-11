using ApiModels.Auth;
using Services;
using Services.Base;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public class AuthServiceTests : BaseServiceTests
    {
        private readonly AuthService authService;
        private RegisterRequestModel registerModel = new RegisterRequestModel()
        {
            Password = "correctPassword",
            RepeatPassword = "correctPassword",
        };

        public AuthServiceTests()
        {
            this.authService = new AuthService(null, this.jsonService);
        }

        [Fact]
        public async Task Register_Returns_Not_Equal_Passwords()
        {
            //Arrange
            this.registerModel.Password = "wrong";
            this.response = new ServiceResponseModel(this.GetErrors().NotEqualPasswords, null);

            //Act
            ServiceResponseModel result = await this.authService.Register(this.registerModel);

            //Assert
            Assert.Null(result.Success);
            Assert.Null(result.RawData);
            Assert.Equal(response, result);
        }

        //[Fact]
        //public async Task Register_Returns_Existing_Username()
        //{
        //    //Arrange
        //    this.registerModel.Username = "existing";

        //    ServiceResponseModel response = new ServiceResponseModel(this.jsonService.Deserialiaze<ErrorMessagesModel>(Paths.ErrorMessages).ExistingUserName, null);

        //    //Act
        //    ServiceResponseModel result = await this.authService.Register(this.registerModel);

        //    //Assert
        //    Assert.Null(result.Success);
        //    Assert.Null(result.RawData);
        //    Assert.Equal(response, result);
        //}
    }
}
