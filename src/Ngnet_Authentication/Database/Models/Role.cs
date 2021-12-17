using Common.Enums;
using Database.Models.Base;
using System;

namespace Database.Models
{
    public class Role : BaseModel<string>
    {
        public Role(RoleTitle title)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Title = title;
        }

        public RoleTitle Title { get; set; }
    }
}
