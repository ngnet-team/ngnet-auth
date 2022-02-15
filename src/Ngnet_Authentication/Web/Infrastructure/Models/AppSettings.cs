using Services.Seeding.Models;

namespace Web.Infrastructure.Models
{
    public class AppSettings
    {
        public ApplicationCall[] ApplicationCalls { get; set; }

        public string ApiKey { get; set; }

        public SeedingModel Seeding { get; set; }
    }
}
