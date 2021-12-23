using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Common;
using Common.Enums;
using Database.Models.Base;

namespace Database.Models
{
    public class User : BaseModel<string>
    {
        public User()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Experiences = new HashSet<Entry>();
        }

        public string RoleId { get; set; }

        [MinLength(Global.EmailMinLength), MaxLength(Global.EmailMaxLength)]
        public string Email { get; set; }

        [MinLength(Global.UsernameMinLength), MaxLength(Global.UsernameMaxLength)]
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        [MinLength(Global.NameMinLength), MaxLength(Global.NameMaxLength)]
        public string FirstName { get; set; }

        [MinLength(Global.NameMinLength), MaxLength(Global.NameMaxLength)]
        public string LastName { get; set; }

        public GenderType? Gender { get; set; }

        [Range(Global.AgeMin, Global.AgeMax)]
         public int? Age { get; set; }

        public ICollection<Entry> Experiences { get; set; }
    }
}
