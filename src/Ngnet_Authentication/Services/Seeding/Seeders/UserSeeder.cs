using System.Linq;
using System.Threading.Tasks;

using Common;
using Common.Enums;
using Database;
using Database.Models;

namespace Services.Seeding.Seeder
{
    public class UserSeeder : ISeeder
    {
        private readonly UserSeederModel[] owners;
        private readonly UserSeederModel[] admins;
        private NgnetAuthDbContext database;

        public UserSeeder(UserSeederModel[] owners, UserSeederModel[] admins)
        {
            this.owners = owners;
            this.admins = admins;
        }

        public async Task SeedAsync(NgnetAuthDbContext database)
        {
            this.database = database;

            if (this.owners != null)
            {
                foreach (var owner in this.owners)
                {
                    await this.SeedUser(owner, RoleType.Owner);
                }
            }

            if (this.admins != null)
            {
                foreach (var admin in this.admins)
                {
                    await this.SeedUser(admin, RoleType.Admin);
                }
            }
        }

        private async Task SeedUser(UserSeederModel u, RoleType roleType)
        {
            User user = this.database.Users.FirstOrDefault(x => x.Username == u.Username);

            if (user != null)
                return;

            Role role = this.database.Roles.FirstOrDefault(x => x.Type == roleType);

            Address address = new Address();
            await this.database.Addresses.AddAsync(address);

            Contact contact = new Contact();
            await this.database.Contacts.AddAsync(contact);

            user = new User()
            {
                RoleId = role.Id,
                Email = u.Email,
                Username = u.Username,
                PasswordHash = Hash.CreatePassword(u?.Password),
                //Optional
                FirstName = u?.FirstName,
                MiddleName = u?.MiddleName,
                LastName = u?.LastName,
                Gender = Global.GetGender(u?.Gender),
                Age = u?.Age,
                AddressId = address.Id,
                ContactId = contact.Id,
            };
            await this.database.Users.AddAsync(user);
        }
    }
}
