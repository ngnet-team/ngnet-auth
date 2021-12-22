using ApiModels.Owners;
using Services.Admins;
using System.Threading.Tasks;

namespace Services.Owners
{
    public interface IOwnerService : IAdminService
    {
        public Task<ServiceResponseModel> SetRoleMembers(MaxRoles maxRoles);
    }
}
