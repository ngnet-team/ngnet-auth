﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using ApiModels.Common;
using ApiModels.Guest;
using Common;
using Common.Json.Service;
using Common.Enums;
using Services.Base;
using Web.Infrastructure.Models;

namespace Web.Controllers.Base
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected readonly JsonService jsonService;
        protected readonly IConfiguration configuration;
        protected ServiceResponseModel response;

        protected ApiController(JsonService jsonService, IConfiguration configuration)
        {
            this.jsonService = jsonService;
            this.AppSettings = configuration.Get<AppSettings>();
        }

        protected ClaimModel Claims => this.GetClaims();

        protected AppSettings AppSettings { get; }

        protected abstract RoleType RoleRequired { get; }

        protected ClientErrorModel GetErrors()
        {
            return this.jsonService.Deserialiaze<ClientErrorModel>(Paths.ClientErrors);
        }

        protected ClientSuccessModel GetSuccessMsg()
        {
            return this.jsonService.Deserialiaze<ClientSuccessModel>(Paths.ClientSuccess);
        }

        // ------------------- Private -------------------

        private ClaimModel GetClaims()
        {
            var items = this.HttpContext.Items;
            return new ClaimModel((RoleType)items.FirstOrDefault(x => x.Key.Equals("RoleType")).Value)
            {
                UserId = (string)items.FirstOrDefault(x => x.Key.Equals("UserId")).Value,
                Username = (string)items.FirstOrDefault(x => x.Key.Equals("Username")).Value,
            };
        }
    }
}