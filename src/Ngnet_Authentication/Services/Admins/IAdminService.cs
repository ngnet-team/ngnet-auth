using ApiModels.Admins;
using Services.Users;
using System.Threading.Tasks;

namespace Services.Admins
{
    public interface IAdminService : IUserService
    {
        public Task<ServiceResponseModel> ChangeRole(AdminRequestModel model);

        public AdminResponseModel[] GetUsers(int count = 10000);

        public T GetUserIncludedDeleted<T>(string userId);
    }
}
