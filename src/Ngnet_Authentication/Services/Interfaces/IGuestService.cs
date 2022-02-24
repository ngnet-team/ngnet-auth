using System.Threading.Tasks;

using ApiModels.Guest;
using ApiModels.Dtos;
using Common.Enums;
using Database.Models;
using Services.Base;

namespace Services.Interfaces
{
    public interface IGuestService
    {
        public Task<ServiceResponseModel> Register(RegisterRequestModel model);

        public Task<ServiceResponseModel> Login(LoginRequestModel model);

        public Task<ServiceResponseModel> ResetPassword(string email);

        public string CreateJwtToken(JwtTokenModel tokenModel);

        public Task<ServiceResponseModel> AddEntry(Entry exp);

        public UserDto GetUserDtoById(string id, bool allowDeleted = false);

        public UserDto GetUserDtoByUsername(string username, bool allowDeleted = false);

        public RoleType? GetUserRoleType(string userId);
    }
}
