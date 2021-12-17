using ApiModels.Auth;
using ApiModels.Users;
using Common.Enums;
using Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Auth
{
    public interface IAuthService
    {
        public Task<CRUD> Register(RegisterRequestModel model);

        public Task<CRUD> Login(LoginRequestModel model);

        public string CreateJwtToken(string userId, string username, string secret);

        public Task<CRUD> Logout(string userId);

        public UserResponseModel Profile(string userId);

        public Task<CRUD> Update<T>(T model);

        public Task<CRUD> AddExperience(UserExperience exp);

        public ICollection<ExperienceModel> GetExperiences(string UserId);

        public User GetUser(string userId);

        public bool ValidEmail(UserChangeModel model, User user);

        public bool ValidPassword(UserChangeModel model, User user);
    }
}
