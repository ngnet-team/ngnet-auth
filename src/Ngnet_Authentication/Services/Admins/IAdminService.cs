using ApiModels.Admins;
using Common.Enums;
using Services.Auth;
using System.Threading.Tasks;

namespace Services.Admins
{
    public interface IAdminService : IAuthService
    {
        public bool HasPermissions(AdminRequestModel model);

        public Task<CRUD> ChangeRole(AdminRequestModel model);

        public AdminResponseModel[] GetUsers(int count = 10000);
    }
}
