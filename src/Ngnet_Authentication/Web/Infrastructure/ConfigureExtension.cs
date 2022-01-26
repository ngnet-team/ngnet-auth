using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Database;
using Services.Seeding;

namespace Web.Infrastructure
{
    public static class ConfigureExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app, IConfiguration configuration)
        {
            using var servicesScope = app.ApplicationServices.CreateScope();

            var dbContext = servicesScope.ServiceProvider.GetService<NgnetAuthDbContext>();

            dbContext.Database.Migrate();
            
            var owners = configuration.GetSection("Owners").Get<UserSeederModel[]>();
            var admins = configuration.GetSection("Admins").Get<UserSeederModel[]>();
            new DatabaseSeeder(owners, admins).SeedAsync(dbContext).GetAwaiter().GetResult();
        }
    }
}