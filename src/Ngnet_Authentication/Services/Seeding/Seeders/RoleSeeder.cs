﻿using ApiModels.Owners;
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
        private readonly MaxRoles maxRoles;

        public RoleSeeder(MaxRoles maxRoles)
        {
            this.maxRoles = maxRoles;
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
                return new Role(roleType) 
                { 
                    MaxCount = this.maxRoles.Owners
                };
            }
            else if (RoleType.Admin.Equals(roleType))
            {
                return new Role(roleType)
                {
                    MaxCount = this.maxRoles.Admins
                };
            }
            else
            {
                return new Role(roleType);
            }
        }
    }
}
