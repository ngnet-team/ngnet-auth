using Common.Json.Service;
using Database;
using Services.Admins;

namespace Services.Owners
{
    public class OwnerService : AdminService, IOwnerService
    {
        public OwnerService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }
    }
}
