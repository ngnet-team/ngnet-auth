﻿using AutoMapper;
using Database.Models;
using Mapper;
using System;

namespace ApiModels.Admins
{
    public class RoleResponseModel : IMapFrom<Role>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int? MaxCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool IsDeleted { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Role, RoleResponseModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Title.ToString())); // Do not work??
        }
    }
}