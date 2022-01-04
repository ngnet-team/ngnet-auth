using System.Threading.Tasks;
using System.Collections.Generic;

using ApiModels.Admins;
using Services.Base;
using ApiModels.Dtos;

namespace Services.Interfaces
{
    public interface IAdminService : IUserService
    {
        public Task<ServiceResponseModel> ChangeRole(AdminRequestModel model);

        public AdminResponseModel[] GetUsers(int? count = null);

        public UserDto GetDeletableUser(string userId);

        public RoleResponseModel[] GetRoles(int? count = null);

        public ICollection<EntryModel> GetEntries(string userId);
    }
}
