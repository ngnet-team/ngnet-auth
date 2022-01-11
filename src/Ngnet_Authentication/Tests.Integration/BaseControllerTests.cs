using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using ApiModels.Auth;
using Database;
using Web;
using Services.Base;
using ApiModels;
using Common.Json.Service;
using Common;

namespace Tests.Integration
{
    public class BaseControllerTests
    {
        private readonly JsonService jsonService;
        private const string AppUrl = @"https://localhost:5000/";

        protected BaseControllerTests()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(b => 
                {
                    b.ConfigureServices(s => 
                    {
                        var dbService = s.FirstOrDefault(x => x.ServiceType == typeof(NgnetAuthDbContext));
                        s.Remove(dbService);
                        s.AddDbContext<NgnetAuthDbContext>(opt => opt.UseInMemoryDatabase("DemoDb"));
                    });
                });

            this.Http = appFactory.CreateClient();

            this.jsonService = new JsonService();
        }

        protected ServiceResponseModel Response;

        protected HttpClient Http { get; }

        protected string Url(string controller, string method)
        {
            return AppUrl + controller + "/" + method;
        }

        protected async Task AuthenticateAsync()
        {
            this.Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", await this.GetJwtAsync());
        }

        protected ErrorMessagesModel GetErrors()
        {
            return this.jsonService.Deserialiaze<ErrorMessagesModel>(Paths.ErrorMessages);
        }

        protected SuccessMessagesModel GetSuccessMsg()
        {
            return this.jsonService.Deserialiaze<SuccessMessagesModel>(Paths.SuccessMessages);
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await this.Http.PostAsJsonAsync(this.Url("auth", "login"), new LoginRequestModel() 
            {
                Username = "dsotirov",
                Password = "dsotirov123",
            });

            var loginResponse = await response.Content.ReadAsAsync<LoginResponseModel>();
            return loginResponse.Token;
        }
    }
}
