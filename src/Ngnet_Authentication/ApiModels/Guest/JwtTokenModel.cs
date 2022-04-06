namespace ApiModels.Guest
{
    public class JwtTokenModel
    {
        public JwtTokenModel(string secretKey)
        {
            this.SecretKey = secretKey;
        }

        public string Issuer { get; set; }

        public string SecretKey { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public string RoleName { get; set; }
    }
}
