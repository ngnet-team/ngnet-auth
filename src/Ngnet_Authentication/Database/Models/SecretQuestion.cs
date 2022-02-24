using Database.Models.Base;

namespace Database.Models
{
    public class SecretQuestion : BaseModel<int>
    {
        public string UserId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }
}
