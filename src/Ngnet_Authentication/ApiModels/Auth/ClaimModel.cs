using Common.Enums;

namespace ApiModels.Auth
{
    public class ClaimModel
    {
        public ClaimModel(RoleType roleType = RoleType.Auth)
        {
            this.RoleType = roleType;
        }

        public string UserId { get; set; }

        public string Username { get; set; }

        public RoleType RoleType { get; }
    }
}
