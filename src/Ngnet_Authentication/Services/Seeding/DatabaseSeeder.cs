using ApiModels.Owners;
using Common.Enums;
using Database;
using Services.Seeding.Models;
using Services.Seeding.Seeder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Seeding
{
    public class DatabaseSeeder : ISeeder
    {
        private readonly ICollection<RoleModel> roleModels;
        private readonly SeedingModel seeding;

        public DatabaseSeeder(SeedingModel seeding)
        {
            this.roleModels = new HashSet<RoleModel>();
            this.seeding = seeding;
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
                new UserSeeder(this.seeding),
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
                Name = RoleType.Owner.ToString(),
                MaxCount = this.seeding?.Owners?.Length,
            });
            this.roleModels.Add(new RoleModel()
            {
                Name = RoleType.Admin.ToString(),
                MaxCount = this.seeding?.Admins?.Length,
            });
        }
    }
}
