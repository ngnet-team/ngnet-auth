using System;

namespace Database.Models
{
    public class Entry
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public bool Login { get; set; } // Login = true/Logout = false

        public DateTime CreatedOn { get; set; }
    }
}
