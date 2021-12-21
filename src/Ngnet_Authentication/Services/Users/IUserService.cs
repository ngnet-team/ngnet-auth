using Services.Auth;
using System.Threading.Tasks;

namespace Services.Users
{
    public interface IUserService : IAuthService
    {
        public T Profile<T>(string userId);
    }
}
