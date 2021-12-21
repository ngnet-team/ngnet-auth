﻿using ApiModels.Auth;
using ApiModels.Users;
using System.Collections.Generic;

namespace ApiModels.Admins
{
    public class AdminRequestModel : UserRequestModel
    {
        public AdminRequestModel()
        {
            this.Experiances = new HashSet<ExperienceModel>();
        }

        public string RoleName { get; set; }

        public string CreatedOn { get; set; }

        public ICollection<ExperienceModel> Experiances { get; set; }

        public string ModifiedOn { get; set; }

        public string DeletedOn { get; set; }

        public bool IsDeleted { get; set; }

        public bool PermanentDeletion { get; set; }
    }
}
