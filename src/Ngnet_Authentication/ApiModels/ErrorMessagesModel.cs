using Common.Json.Models;

namespace ApiModels
{
    public class ErrorMessagesModel
    {
        public LanguagesModel NotEqualPasswords { get; set; }

        public LanguagesModel InvalidEmail { get; set; }

        public LanguagesModel InvalidUsername { get; set; }

        public LanguagesModel InvalidPasswords { get; set; }

        public LanguagesModel UserNotFound { get; set; }

        public LanguagesModel UsersNotFound { get; set; }

        public LanguagesModel ExistingUserName { get; set; }

        public LanguagesModel VehicleCareNotFound { get; set; }

        public LanguagesModel VehicleCaresNotFound { get; set; }

        public LanguagesModel NoPermissions { get; set; }

        public LanguagesModel VehicleCareNamesNotFound { get; set; }

        public LanguagesModel CompanyNamesNotFound { get; set; }

        public LanguagesModel HealthCareNamesNotFound { get; set; }

        public LanguagesModel HealthCareNotFound { get; set; }

        public LanguagesModel HealthCaresNotFound { get; set; }

        public LanguagesModel NotEqualFields { get; set; }
    }
}
