using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Database;
using Services.Seeding.Models;
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

            SeedingModel seeding = configuration.GetSection("Seeding").Get<SeedingModel>();
            if (seeding != null)
            {
                new DatabaseSeeder(seeding).SeedAsync(dbContext).GetAwaiter().GetResult();
            }
        }
    }
}