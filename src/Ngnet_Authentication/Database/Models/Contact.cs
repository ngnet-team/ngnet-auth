using System;

using Database.Models.Base;

namespace Database.Models
{
    public class Contact : BaseModel<string>
    {
        public Contact()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string Facebook { get; set; }

        public string Instagram { get; set; }

        public string TikTok { get; set; }

        public string Youtube { get; set; }

        public string Twitter { get; set; }
    }
}
