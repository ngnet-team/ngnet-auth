using System.ComponentModel.DataAnnotations;

namespace ApiModels.Auth
{
    public abstract class UserChangeModel
    {
        [Required]
        public string Old { get; set; }

        [Required]
        public string New { get; set; }

        [Required]
        public string RepeatNew { get; set; }
    }
}
