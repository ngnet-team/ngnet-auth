using ApiModels.Owners;
using Services.Base;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOwnerService : IAdminService
    {
        public Task<ServiceResponseModel> SetRoleMembers(MaxRoles maxRoles);
    }
}
