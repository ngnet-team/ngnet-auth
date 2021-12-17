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
            if (user == null)
            {
                //user = MappingFactory.Mapper.Map<User>(u);
                //user.CreatedOn = DateTime.UtcNow;
                user = new User()
                {
                    Email = u.Email,
                    Username = u.Username,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    CreatedOn = DateTime.UtcNow,
                };

                await this.database.Users.AddAsync(user);

                Role role = this.database.Roles.FirstOrDefault(x => x.Title == roleTitle);
                await this.database.UserRoles.AddAsync(new UserRole()
                {
                    User = user,
                    Role = role
                });
            }
        }
    }
}
