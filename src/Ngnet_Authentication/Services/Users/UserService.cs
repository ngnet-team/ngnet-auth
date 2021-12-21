using Common.Enums;
using Common.Json.Service;
using Database;
using Mapper;
using Services.Auth;
using System.Linq;

namespace Services.Users
{
    public class UserService : AuthService, IUserService
    {
        public UserService(NgnetAuthDbContext database, JsonService jsonService)
            : base(database, jsonService)
        {
        }

        public override RoleTitle RoleTitle { get; set; } = RoleTitle.User;

        public T Profile<T>(string userId)
        {
            return this.database.Users
                .Where(x => x.Id == userId)
                .Where(x => !x.IsDeleted)
                .To<T>()
                .FirstOrDefault();
        }
    }
}
