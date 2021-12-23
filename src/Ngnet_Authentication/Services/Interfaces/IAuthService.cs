using System.Threading.Tasks;

using ApiModels.Auth;
using Common.Enums;
using Database.Models;
using Services.Base;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        public Task<ServiceResponseModel> Register(RegisterRequestModel model);

        public Task<ServiceResponseModel> Login(LoginRequestModel model);

        public string CreateJwtToken(JwtTokenModel tokenModel);

        public Task<ServiceResponseModel> AddEntry(Entry exp);

        public Task<ServiceResponseModel> Update<T>(T model);

        public User GetUserById(string id);

        public User GetUserByUsername(string username);

        public RoleTitle GetUserRole(User user);

        public Role GetRoleByString(string roleName);

        public Role GetRole(RoleTitle roleTitle);
    }
}
