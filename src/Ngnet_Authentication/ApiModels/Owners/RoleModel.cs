using System;

namespace ApiModels.Owners
{
    public class RoleModel
    {
        public string RoleName { get; set; }

        public int? MaxCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool isDeleted { get; set; }
    }
}
