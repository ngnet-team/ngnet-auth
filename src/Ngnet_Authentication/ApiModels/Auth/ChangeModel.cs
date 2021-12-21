using System.ComponentModel.DataAnnotations;

namespace ApiModels.Auth
{
    public abstract class ChangeModel
    {
        [Required]
        public string Old { get; set; }

        [Required]
        public string New { get; set; }

        [Required]
        public string RepeatNew { get; set; }
    }
}
