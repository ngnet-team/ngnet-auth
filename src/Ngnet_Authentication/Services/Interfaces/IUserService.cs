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

        public ServiceResponseModel Change(UserChangeModel model, User user);
    }
}
