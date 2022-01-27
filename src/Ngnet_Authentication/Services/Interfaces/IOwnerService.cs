using System.Threading.Tasks;

using ApiModels.Owners;
using Services.Base;

namespace Services.Interfaces
{
    public interface IOwnerService : IAdminService
    {
        public Task<ServiceResponseModel> SetMaxRoles(RoleModel[] models);
    }
}
