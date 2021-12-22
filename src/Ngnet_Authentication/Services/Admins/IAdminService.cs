﻿using ApiModels.Admins;
using Services.Users;
using System.Threading.Tasks;

namespace Services.Admins
{
    public interface IAdminService : IUserService
    {
        public Task<ServiceResponseModel> ChangeRole(AdminRequestModel model);

        public AdminResponseModel[] GetUsers(int? count = null);

        public RoleResponseModel[] GetRoles(int? count = null);
    }
}
