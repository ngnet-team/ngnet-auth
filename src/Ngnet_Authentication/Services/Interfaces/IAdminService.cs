using System.Threading.Tasks;

using ApiModels.Admins;
using Services.Base;

namespace Services.Interfaces
{
    public interface IAdminService : IMemberService
    {
        public Task<ServiceResponseModel> ChangeRole(AdminRequestModel model, string currUser);

        public RoleModel[] GetRoles();

        public EntryModel[] GetEntries(string userId = null);

        public RightsChangeModel[] GetRightsChanges(string author = null);
    }
}
