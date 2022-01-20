using ApiModels.Users;

namespace Web.Infrastructure.Models
{
    public class AppSettings
    {
        public ApplicationCall[] ApplicationCalls { get; set; }

        public UserSeederModel[] Owners { get; set; }

        public UserSeederModel[] Admins { get; set; }
    }
}
