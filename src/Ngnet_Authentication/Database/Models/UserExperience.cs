using System;

namespace Database.Models
{
    public class UserExperience
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public DateTime? LoggedIn { get; set; }

        public DateTime? LoggedOut { get; set; }
    }
}
