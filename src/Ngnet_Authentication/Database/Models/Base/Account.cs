using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Common;

namespace Database.Models.Base
{
    public class Account : BaseModel<string>
    {
        public Account()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Entries = new HashSet<Entry>();
        }

        public string RoleId { get; set; }

        [MinLength(Global.EmailMinLength), MaxLength(Global.EmailMaxLength)]
        public string Email { get; set; }

        [MinLength(Global.UsernameMinLength), MaxLength(Global.UsernameMaxLength)]
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public ICollection<Entry> Entries { get; set; }
    }
}
