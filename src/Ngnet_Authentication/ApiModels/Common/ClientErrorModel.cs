using Common.Json.Models;

namespace ApiModels.Common
{
    public class ClientErrorModel
    {
        public ResponseMessage InvalidEmail { get; set; }

        public ResponseMessage InvalidUsername { get; set; }

        public ResponseMessage InvalidPassword { get; set; }

        public ResponseMessage RequiredEmail { get; set; }

        public ResponseMessage RequiredUsername { get; set; }

        public ResponseMessage RequiredPassword { get; set; }

        public ResponseMessage InvalidEmailLength { get; set; }

        public ResponseMessage InvalidUsernameLength { get; set; }

        public ResponseMessage InvalidPasswordLength { get; set; }

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
