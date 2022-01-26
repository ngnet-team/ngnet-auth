using ApiModels.Owners;
using Common.Enums;
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
        private readonly ICollection<RoleModel> roleModels;

        public DatabaseSeeder(UserSeederModel[] owners, UserSeederModel[] admins)
        {
            this.owners = owners;
            this.admins = admins;
            this.roleModels = new HashSet<RoleModel>();
        }

        public async Task SeedAsync(NgnetAuthDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            this.PrepareSeeders();

            var seeders = new List<ISeeder>
            {
                new RoleSeeder(this.roleModels),
                new UserSeeder(this.owners, this.admins),
            };

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(dbContext);
                await dbContext.SaveChangesAsync();
            }
        }

        private void PrepareSeeders() 
        {
            this.roleModels.Add(new RoleModel()
            {
                RoleName = RoleType.Owner.ToString(),
                MaxCount = this.owners?.Length,
            });
            this.roleModels.Add(new RoleModel()
            {
                RoleName = RoleType.Admin.ToString(),
                MaxCount = this.admins?.Length,
            });
        }
    }
}
