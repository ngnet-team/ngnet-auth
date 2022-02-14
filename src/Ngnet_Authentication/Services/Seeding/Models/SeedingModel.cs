namespace Services.Seeding.Models
{
    public class SeedingModel
    {
        public UserSeederModel[] Owners { get; set; }

        public UserSeederModel[] Admins { get; set; }

        public UserSeederModel[] Members { get; set; }

        public UserSeederModel[] Users { get; set; }
    }
}
