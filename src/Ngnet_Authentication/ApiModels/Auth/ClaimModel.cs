using Common.Enums;

namespace ApiModels.Auth
{
    public class ClaimModel
    {
        public ClaimModel(RoleTitle roleTitle = RoleTitle.Guest)
        {
            this.RoleTitle = roleTitle;
        }

        public string UserId { get; set; }

        public string Username { get; set; }

        public RoleTitle RoleTitle { get; }
    }
}
