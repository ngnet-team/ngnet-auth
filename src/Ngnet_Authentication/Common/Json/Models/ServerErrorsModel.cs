namespace Common.Json.Models
{
    public class ServerErrorsModel
    {
        public ResponseMessage InvalidApiKey { get; set; }

        public ResponseMessage MissingBody { get; set; }

        public ResponseMessage MissingToken { get; set; }

        public ResponseMessage InvalidSecretKey { get; set; }

        public ResponseMessage InvalidToken { get; set; }

        public ResponseMessage IvalidRole { get; set; }

        public ResponseMessage InvalidUser { get; set; }

        public ResponseMessage TokenExpired { get; set; }

        public ResponseMessage NotParsedAction { get; set; }

        public ResponseMessage LoggoutFirst { get; set; }
    }
}
