using System.Collections.Generic;

using ApiModels.Users;

namespace ApiModels.Admins
{
    public class AdminResponseModel : UserResponseModel
    {
        public AdminResponseModel()
        {
            this.Entries = new HashSet<EntryModel>();
        }

        public string Id { get; set; }

        public string RoleName { get; set; }

        public ICollection<EntryModel> Entries { get; set; }

        public string ModifiedOn { get; set; }

        public string DeletedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
