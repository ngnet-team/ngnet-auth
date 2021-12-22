using ApiModels.Owners;
using Database;
using Services.Seeding.Seeder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Seeding
{
    public class DatabaseSeeder : ISeeder
    {
        private readonly UserSeederModel[] owners;
        private readonly UserSeederModel[] admins;

        public DatabaseSeeder(UserSeederModel[] owners, UserSeederModel[] admins)
        {
            this.owners = owners;
            this.admins = admins;
        }

        public async Task SeedAsync(NgnetAuthDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            var seeders = new List<ISeeder>
            {
                new RoleSeeder(new MaxRoles(this.owners.Length, this.admins.Length)),
                new UserSeeder(this.owners, this.admins),
            };

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(dbContext);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
