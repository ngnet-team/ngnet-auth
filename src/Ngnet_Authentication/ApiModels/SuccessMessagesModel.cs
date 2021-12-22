using Common.Json.Models;

namespace ApiModels
{
    public class SuccessMessagesModel
    {
        public LanguagesModel Registered { get; set; }

        public LanguagesModel LoggedIn { get; set; }

        public LanguagesModel LoggedOut { get; set; }

        public LanguagesModel Created { get; set; }

        public LanguagesModel Updated { get; set; }

        public LanguagesModel Deleted { get; set; }

        public LanguagesModel AlreadyStored { get; set; }
    }
}
