using Common.Enums;
using Database.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class User : BaseModel<string>
    {
        public User()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(250)]
        public string PasswordHash { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public string Gender { get; set; }

        [Range(0, 120)]
         public GenderType? Age { get; set; }

        public ICollection<UserExperience> Experiences { get; set; }
    }
}
