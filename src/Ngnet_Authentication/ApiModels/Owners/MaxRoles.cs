using Common.Enums;

namespace ApiModels.Owners
{
    public class MaxRoles
    {
        public MaxRoles(int owners, int admins)
        {
            this.Owners = owners;
            this.Admins = admins;
        }

        public int Owners { get; set; }

        public int Admins { get; set; }

        public int? Users { get; set; }

        public int? Guests { get; set; }
    }
}
