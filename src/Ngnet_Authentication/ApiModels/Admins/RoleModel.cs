using System;

using Database.Models;
using Mapper;

namespace ApiModels.Admins
{
    public class RoleModel : IMapFrom<Role>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int? MaxCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
