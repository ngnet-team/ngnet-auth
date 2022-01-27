using System.Threading.Tasks;

using ApiModels.Users;
using Services.Base;

namespace Services.Interfaces
{
    public interface IUserService : IAuthService
    {
        public T Profile<T>(string userId);

        public Task<ServiceResponseModel> Logout(string userId, string username);

        public Task<ServiceResponseModel> Delete(string userId);

        public Task<ServiceResponseModel> DeleteAccount(string userId);

        public Task<ServiceResponseModel> ResetPassword(string userId);

        public Task<ServiceResponseModel> Update(UpdateRequestModel model);

        public Task<ServiceResponseModel> Change(ChangeRequestModel model);
    }
}
