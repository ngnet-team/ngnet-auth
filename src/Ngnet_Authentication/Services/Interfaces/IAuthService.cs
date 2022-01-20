using System.Threading.Tasks;

using ApiModels.Auth;
using ApiModels.Dtos;
using ApiModels.Users;
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

        public UserDto GetUserById(string id);

        public UserDto GetUserByUsername(string username);

        public Role GetUserRole(UserDto userDto);

        public Role GetRoleByString(string roleName);

        public Role GetRoleByEnum(RoleType roleType);
    }
}
