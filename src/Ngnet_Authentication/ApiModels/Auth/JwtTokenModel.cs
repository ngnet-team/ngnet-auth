namespace ApiModels.Auth
{
    public class JwtTokenModel
    {
        public JwtTokenModel(string secretKey)
        {
            this.SecretKey = secretKey;
        }

        public string UserId { get; set; }

        public string Username { get; set; }

        public string RoleName { get; set; }

        public string SecretKey { get; set; }
    }
}
