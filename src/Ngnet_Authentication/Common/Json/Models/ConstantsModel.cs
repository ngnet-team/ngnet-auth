namespace Common.Json.Models
{
    public class ConstantsModel
    {
        public string CookieKey { get; set; }

        public int TokenExpires { get; set; }

        public string AuthHeaderKey { get; set; }

        public string AuthHeaderPreValue { get; set; }

        public string ApiKeyHeaderKey { get; set; }

        public int HashBytes { get; set; }

        public string EmailPattern { get; set; }
    }
}
