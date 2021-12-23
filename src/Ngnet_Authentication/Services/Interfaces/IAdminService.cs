﻿using System.Threading.Tasks;
using System.Collections.Generic;

using ApiModels.Admins;
using Database.Models;
using Services.Base;

namespace Services.Interfaces
{
    public interface IAdminService : IUserService
    {
        public Task<ServiceResponseModel> ChangeRole(AdminRequestModel model);

        public AdminResponseModel[] GetUsers(int? count = null);

        public User GetDeletableUser(string userId);

        public RoleResponseModel[] GetRoles(int? count = null);

        public ICollection<EntryModel> GetEntries(string userId);

        public Task<ServiceResponseModel> DeleteUser(User user);
    }
}
