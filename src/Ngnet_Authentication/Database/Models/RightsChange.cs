using Common.Enums;
using System;

namespace Database.Models
{
    public class RightsChange
    {
        public int Id { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public RoleType Role { get; set; }

        public DateTime Date { get; set; }
    }
}
