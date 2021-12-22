using Common.Enums;
using System;

namespace ApiModels.Owners
{
    public class MaxRoles
    {
        public int? Owners { get; set; }

        public int? Admins { get; set; }

        public int? Users { get; set; }

        public int? Guests { get; set; }

        public (RoleTitle?, int?) Get()
        {
            return this.Owners != null ? (RoleTitle.Owner, this.Owners) :
                  this.Admins != null ? (RoleTitle.Admin, this.Admins) :
                  this.Users != null ? (RoleTitle.User, this.Users) :
                  this.Guests != null ? (RoleTitle.Guest, this.Guests) :
                  (null, null);
        }
    }
}
