using Common.Json.Models;

namespace ApiModels
{
    public class ErrorMessagesModel
    {
        public LanguagesModel NotEqualPasswords { get; set; }

        public LanguagesModel InvalidEmail { get; set; }

        public LanguagesModel InvalidUsername { get; set; }

        public LanguagesModel InvalidPassword { get; set; }

        public LanguagesModel UserNotFound { get; set; }

        public LanguagesModel UsersNotFound { get; set; }

        public LanguagesModel ExistingUserName { get; set; }

        public LanguagesModel NoPermissions { get; set; }

        public LanguagesModel NotEqualFields { get; set; }

        public LanguagesModel InvalidRole { get; set; }

        public LanguagesModel AlreadyLoggedIn { get; set; }

        public LanguagesModel InvalidName { get; set; }

        public LanguagesModel MissingBody { get; set; }
    }
}
