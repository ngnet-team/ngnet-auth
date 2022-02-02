using System;
using System.ComponentModel.DataAnnotations;

using Common;
using Common.Enums;
using Database.Models.Base;

namespace Database.Models
{
    public class User : Account
    {
        public User()
        {
        }

        [MinLength(Global.NameMinLength), MaxLength(Global.NameMaxLength)]
        public string FirstName { get; set; }

        [MinLength(Global.NameMinLength), MaxLength(Global.NameMaxLength)]
        public string MiddleName { get; set; }

        [MinLength(Global.NameMinLength), MaxLength(Global.NameMaxLength)]
        public string LastName { get; set; }

        public GenderType? Gender { get; set; }

        [Range(Global.AgeMin, Global.AgeMax)]
        public int? Age { get; set; }

        public string AddressId { get; set; }

        public string ContactId { get; set; }
    }
}
