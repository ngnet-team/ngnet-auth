using Common.Enums;

namespace ApiModels.Owners
{
    public class MaxRoles
    {
        public int? Owners { get; set; }

        public int? Admins { get; set; }

        public int? Users { get; set; }

        public int? Guests { get; set; }

        public (RoleType?, int?) Get()
        {
            return this.Owners != null ? (RoleType.Owner, this.Owners) :
                  this.Admins != null ? (RoleType.Admin, this.Admins) :
                  this.Users != null ? (RoleType.User, this.Users) :
                  this.Guests != null ? (RoleType.Guest, this.Guests) :
                  (null, null);
        }
    }
}
