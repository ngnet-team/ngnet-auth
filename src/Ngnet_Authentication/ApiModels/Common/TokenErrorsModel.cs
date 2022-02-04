using Common.Json.Models;

namespace ApiModels.Common
{
    public class TokenErrorsModel
    {
        public ResponseMessage InvalidSecretKey { get; set; }

        public ResponseMessage Expired { get; set; }

        public ResponseMessage IvalidRole { get; set; }

        public ResponseMessage InvalidUser { get; set; }
    }
}
