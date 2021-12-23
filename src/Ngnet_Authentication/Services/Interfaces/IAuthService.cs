using ApiModels.Auth;
using ApiModels.Users;
using Common.Enums;
using Database.Models;
using Services.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        public Task<ServiceResponseModel> Register(RegisterRequestModel model);

        public Task<ServiceResponseModel> Login(LoginRequestModel model);

        public string CreateJwtToken(JwtTokenModel tokenModel);

        public Task<ServiceResponseModel> Logout(string userId);

        public Task<ServiceResponseModel> AddExperience(UserExperience exp);

        public ICollection<ExperienceModel> GetExperiences(string UserId);

        public Task<ServiceResponseModel> Update<T>(T model);

        public User GetUserById(string id);

        public User GetUserByUsername(string username);

        public RoleTitle GetUserRole(User user);

        public Role GetRoleByString(string roleName);

        public Role GetRole(RoleTitle roleTitle);

        public bool ValidEmail(UserChangeModel model, User user);

        public bool ValidPassword(UserChangeModel model, User user);
    }
}
