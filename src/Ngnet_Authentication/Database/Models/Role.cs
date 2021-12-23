using System;

using Common.Enums;
using Database.Models.Base;

namespace Database.Models
{
    public class Role : BaseModel<string>
    {
        public Role(RoleType type)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Type = type;
        }

        public RoleType Type { get; set; }

        public int? MaxCount { get; set; }
    }
}
