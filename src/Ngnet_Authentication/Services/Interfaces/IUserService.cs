using System.Threading.Tasks;

using ApiModels.Users;
using Database.Models;
using Services.Base;

namespace Services.Interfaces
{
    public interface IUserService : IAuthService
    {
        public T Profile<T>(string userId);

        public Task<ServiceResponseModel> Logout(string userId);

        public Task<ServiceResponseModel> DeleteAccount(string userId);

        public bool ValidEmail(UserChangeModel model, User user);

        public bool ValidPassword(UserChangeModel model, User user);
    }
}
