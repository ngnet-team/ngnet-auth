using Common.Json.Service;
using Database;
using Services.Interfaces;

namespace Services
{
    public class MemberService : UserService, IMemberService
    {
        public MemberService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }
    }
}
