using Common.Enums;
using Database;
using Database.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Seeding.Seeder
{
    public class RoleSeeder : ISeeder
    {
        public async Task SeedAsync(NgnetAuthDbContext database)
        {
            foreach (var roleTitle in Enum.GetValues<RoleTitle>())
            {
                await SeedRoleAsync(database, roleTitle);
            }
        }

        private async Task SeedRoleAsync(NgnetAuthDbContext database, RoleTitle roleTitle)
        {
            var role = database.Roles.FirstOrDefault(x => x.Title.Equals(roleTitle));
            if (role == null && Enum.IsDefined(roleTitle))
            {
                await database.Roles.AddAsync(new Role(roleTitle));
            }
        }
    }
}
