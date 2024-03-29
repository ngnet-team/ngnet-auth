﻿using Microsoft.EntityFrameworkCore;

using Database.Models;

namespace Database
{
    public class NgnetAuthDbContext : DbContext
    {
        public NgnetAuthDbContext(DbContextOptions<NgnetAuthDbContext> options)
            :base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Entry> Entries { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<RightsChange> RightsChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);
        }
    }
}
