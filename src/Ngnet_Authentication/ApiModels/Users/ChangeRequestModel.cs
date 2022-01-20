using System.ComponentModel.DataAnnotations;

namespace ApiModels.Users
{
    public class ChangeRequestModel
    {
        public string Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Old { get; set; }

        [Required]
        public string New { get; set; }

        [Required]
        public string RepeatNew { get; set; }
    }
}
