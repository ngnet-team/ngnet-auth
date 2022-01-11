using ApiModels.Users;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration
{
    public class UserControllerTests : AuthControllerTests
    {
        public UserControllerTests()
        {

        }

        protected UserResponseModel Response { get; set; }

        [Fact]
        public async Task Profile_Returns_User_Profile()
        {
            //Arrange
            await this.AuthenticateAsync();

            //Act
            var result = this.Http.GetAsync(this.Url("user", "profile"));

            //Assert
            var a = 1;
        }
    }
}
