using System.ComponentModel.DataAnnotations;

namespace ApiModels.Admins
{
    public class AdminRequestModel
    {
        [Required]
        public string Id { get; set; }

        public string RoleName { get; set; }
    }
}
