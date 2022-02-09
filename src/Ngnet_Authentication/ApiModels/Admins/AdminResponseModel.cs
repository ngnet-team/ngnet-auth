using ApiModels.Users;

namespace ApiModels.Admins
{
    public class AdminResponseModel : UserResponseModel
    {
        public string Id { get; set; }

        public string RoleName { get; set; }

        public string ModifiedOn { get; set; }

        public string DeletedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
