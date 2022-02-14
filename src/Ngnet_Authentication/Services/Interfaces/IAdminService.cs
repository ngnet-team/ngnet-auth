using System.Threading.Tasks;

using ApiModels.Admins;
using Services.Base;

namespace Services.Interfaces
{
    public interface IAdminService : IMemberService
    {
        public Task<ServiceResponseModel> ChangeRole(AdminRequestModel model, string currUser);

        public AdminResponseModel[] GetUsers(int? count = null);

        public RoleModel[] GetRoles();

        public EntryModel[] GetEntries(string userId = null);
    }
}
