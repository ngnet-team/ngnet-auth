using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Database
{
    public class DbContextFactory : IDesignTimeDbContextFactory<NgnetAuthDbContext>
    {
        public NgnetAuthDbContext CreateDbContext(string[] args)
        {
            var connectionString = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build()
                .GetSection("ConnectionStrings");

            var builder = new DbContextOptionsBuilder<NgnetAuthDbContext>();
            builder.UseSqlServer(connectionString["DefaultConnection"]);

            return new NgnetAuthDbContext(builder.Options);
        }
    }
}
