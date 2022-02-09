using Common.Json.Models;

namespace ApiModels.Common
{
    public class ClientSuccessModel
    {
        public ResponseMessage Registered { get; set; }

        public ResponseMessage LoggedIn { get; set; }

        public ResponseMessage LoggedOut { get; set; }

        public ResponseMessage Created { get; set; }

        public ResponseMessage Updated { get; set; }

        public ResponseMessage Deleted { get; set; }

        public ResponseMessage AlreadyStored { get; set; }
    }
}
