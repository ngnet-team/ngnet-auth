using Common.Json.Models;

namespace ApiModels
{
    public class SuccessMessagesModel
    {
        public LanguagesModel UserRegistered { get; set; }

        public LanguagesModel UserUpdated { get; set; }

        public LanguagesModel UserLoggedIn { get; set; }

        public LanguagesModel Created { get; set; }

        public LanguagesModel Updated { get; set; }

        public LanguagesModel Deleted { get; set; }
    }
}
