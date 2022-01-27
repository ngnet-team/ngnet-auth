using System.Threading.Tasks;

using ApiModels.Admins;
using Services.Base;
using ApiModels.Dtos;

namespace Services.Interfaces
{
    public interface IAdminService : IMemberService
    {
        public Task<ServiceResponseModel> ChangeRole(AdminRequestModel model);

        public AdminResponseModel[] GetUsers(int? count = null);

        public UserDto GetDeletableUser(string userId);

        public RoleModel[] GetRoles();

        public EntryModel[] GetEntries(string userId = null);
    }
}
