using Common.Json.Models;

namespace ApiModels.Common
{
    public class ClientErrorModel
    {
        public ResponseMessage NotEqualPasswords { get; set; }

        public ResponseMessage InvalidEmail { get; set; }

        public ResponseMessage InvalidUsername { get; set; }

        public ResponseMessage InvalidPassword { get; set; }

        public ResponseMessage InvalidRole { get; set; }

        public ResponseMessage InvalidName { get; set; }

        public ResponseMessage UserNotFound { get; set; }

        public ResponseMessage UsersNotFound { get; set; }

        public ResponseMessage ExistingUserName { get; set; }

        public ResponseMessage NoPermissions { get; set; }

        public ResponseMessage NotEqualFields { get; set; }

        public ResponseMessage AlreadyLoggedIn { get; set; }
    }
}
