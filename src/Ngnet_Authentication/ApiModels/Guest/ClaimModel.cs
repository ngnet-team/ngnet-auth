using Common.Enums;

namespace ApiModels.Guest
{
    public class ClaimModel
    {
        public ClaimModel(RoleType roleType = RoleType.Guest)
        {
            this.RoleType = roleType;
        }

        public string UserId { get; set; }

        public string Username { get; set; }

        public RoleType RoleType { get; }
    }
}
