using Common.Enums;
using Common.Json.Service;
using Database;
using Database.Models;
using Mapper;
using Services.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ServiceResponseModel> DeleteAccount(string userId)
        {
            User user = this.database.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return new ServiceResponseModel(GetErrors().UserNotFound, null);
            
            user.IsDeleted = true;
            user.DeletedOn = DateTime.UtcNow;
            await this.database.SaveChangesAsync();

            return new ServiceResponseModel(null, this.GetSuccessMsg().Deleted);
        }
    }
}
