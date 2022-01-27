using ApiModels.Owners;
using Common.Enums;
using Database;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Seeding.Seeder
{
    public class RoleSeeder : ISeeder
    {
        private readonly ICollection<RoleModel> roleModels;

        public RoleSeeder(ICollection<RoleModel> roleModels)
        {
            this.roleModels = roleModels;
        }

        public async Task SeedAsync(NgnetAuthDbContext database)
        {
            foreach (var roleType in Enum.GetValues<RoleType>())
            {
                await SeedRoleAsync(database, roleType);
            }
        }

        private async Task SeedRoleAsync(NgnetAuthDbContext database, RoleType roleType)
        {
            var role = database.Roles.FirstOrDefault(x => x.Type.Equals(roleType));
            if (role == null && Enum.IsDefined(roleType))
            {
                await database.Roles.AddAsync(this.CreateRole(roleType));
            }
        }

        private Role CreateRole(RoleType roleType)
        {
            if (RoleType.Owner.Equals(roleType))
            {
                int? maxCount = this.roleModels.FirstOrDefault(x => x.RoleName == "Owner")?.MaxCount;
                return new Role(roleType)
                {
                    MaxCount = maxCount
                };
            }
            else if (RoleType.Admin.Equals(roleType))
            {
            int? maxCount = this.roleModels.FirstOrDefault(x => x.RoleName == "Admin")?.MaxCount;
                return new Role(roleType)
                {
                    MaxCount = maxCount
                };
            }
            else
            {
                return new Role(roleType);
            }
        }
    }
}
