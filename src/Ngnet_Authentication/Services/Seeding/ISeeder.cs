using Database;
using System.Threading.Tasks;

namespace Services.Seeding
{
    public interface ISeeder
    {
        Task SeedAsync(NgnetAuthDbContext dbContext);
    }
}
