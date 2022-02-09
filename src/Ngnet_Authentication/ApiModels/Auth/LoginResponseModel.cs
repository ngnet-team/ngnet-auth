using Common.Json.Models;

namespace ApiModels.Auth
{
    public class LoginResponseModel
    {
        public string Token { get; set; }

        public ResponseMessage ResponseMessage { get; set; }
    }
}
