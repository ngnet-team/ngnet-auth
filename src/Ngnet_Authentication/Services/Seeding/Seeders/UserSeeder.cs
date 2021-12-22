using Common;
using Common.Enums;
using Database;
using Database.Models;
using Mapper;
using System;
using System.Linq;
using System.Threading.Tasks;

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

            foreach (var owner in this.owners)
            {
                await this.SeedUser(owner, RoleTitle.Owner);
            }

            foreach (var admin in this.admins)
            {
                await this.SeedUser(admin, RoleTitle.Admin);
            }
        }

        private async Task SeedUser(UserSeederModel u, RoleTitle roleTitle)
        {
            User user = this.database.Users.FirstOrDefault(x => x.Username == u.Username);

            if (user != null)
                return;

            Role role = this.database.Roles.FirstOrDefault(x => x.Title == roleTitle);
            //user = MappingFactory.Mapper.Map<User>(u);
            //user.CreatedOn = DateTime.UtcNow;
            //user.Role = role;
            user = new User()
            {
                Email = u.Email,
                Username = u.Username,
                FirstName = u.FirstName,
                PasswordHash = Hash.CreatePassword(u.Password),
                LastName = u.LastName,
                CreatedOn = DateTime.UtcNow,
                RoleId = role.Id,
            };

            await this.database.Users.AddAsync(user);
        }
    }
}
