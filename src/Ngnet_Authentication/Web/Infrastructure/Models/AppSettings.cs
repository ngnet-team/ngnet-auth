using Services.Seeding.Models;

namespace Web.Infrastructure.Models
{
    public class AppSettings
    {
        public string SecretKey { get; set; }

        public string ApiKey { get; set; }

        public SeedingModel Seeding { get; set; }
    }
}
