using System.ComponentModel.DataAnnotations;

using Common;

namespace ApiModels.Users
{
    public class UserOptionalModel
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        [Range(Global.AgeMin, Global.AgeMax)]
        public int? Age { get; set; }

        public AddressRequestModel Address { get; set; }

        public ContactRequestModel Contact { get; set; }
    }
}
