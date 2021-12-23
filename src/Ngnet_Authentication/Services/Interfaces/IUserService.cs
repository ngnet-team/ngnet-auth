using Services.Base;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService : IAuthService
    {
        public T Profile<T>(string userId);

        public Task<ServiceResponseModel> DeleteAccount(string userId);
    }
}
