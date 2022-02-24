using Common.Json.Models;

namespace ApiModels.Guest
{
    public class LoginResponseModel
    {
        public string Token { get; set; }

        public ResponseMessage ResponseMessage { get; set; }
    }
}
