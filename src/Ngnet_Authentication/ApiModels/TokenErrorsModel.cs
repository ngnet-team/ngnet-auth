using Common.Json.Models;

namespace ApiModels
{
    public class TokenErrorsModel
    {
        public LanguagesModel InvalidSecretKey { get; set; }

        public LanguagesModel Expired { get; set; }

        public LanguagesModel IvalidRole { get; set; }

        public LanguagesModel InvalidUser { get; set; }
    }
}
