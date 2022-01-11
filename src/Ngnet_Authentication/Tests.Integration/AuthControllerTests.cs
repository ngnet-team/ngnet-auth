using ApiModels.Auth;
using FluentAssertions;
using Services.Base;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration
{
    public class AuthControllerTests : BaseControllerTests
    {
        private RegisterRequestModel registerRequest = new RegisterRequestModel()
        {
            Email = "test@test.test",
            Username = "testtest",
            Password = "testtest",
            RepeatPassword = "testtest",
        };

        public AuthControllerTests()
        {
        }

        [Fact]
        public async Task Register_With_Not_Equal_Passwords_Returns_Error()
        {
            //Arrange
            this.registerRequest.Password = "wrong";
            //this.Response = new ServiceResponseModel(this.GetErrors().NotEqualPasswords, null);
            //Act
            //var result = await this.Http.PostAsync<RegisterRequestModel>
            //    (this.Url("auth", "register"), this.registerRequest);

            //Assert
            //result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            //(await result.Content.ReadAsAsync<ServiceResponseModel>()).Should().BeOfType(typeof(ServiceResponseModel));
        }
    }
}
