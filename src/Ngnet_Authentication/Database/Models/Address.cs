using Database.Models.Base;
using System;

namespace Database.Models
{
    public class Address : BaseModel<string>
    {
        public Address()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Country { get; set; }

        public string City { get; set; }

        public string Str { get; set; }
    }
}
