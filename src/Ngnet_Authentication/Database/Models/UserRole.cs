using Database.Models.Base;
using System;

namespace Database.Models
{
    public class UserRole : BaseModel<string>
    {
        public UserRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string UserId { get; set; }

        public User User { get; set; }

        public string RoleId { get; set; }

        public Role Role { get; set; }
    }
}
