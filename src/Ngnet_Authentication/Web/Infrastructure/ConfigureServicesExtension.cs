using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Common.Json.Service;
using Database;
using Mapper;
using Services;
using Services.Email;
using Services.Interfaces;
using Web.Infrastructure.Filters;

namespace Web.Infrastructure
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(c =>
            {
                c.AddProfile(new MappingProfile());
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<NgnetAuthDbContext>(options => 
                options.UseSqlServer(configuration.GetValue<string>("Database:SqlServer:ConnectionString")));
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, ApplicationSettingsModel appSettings)
        {
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            //chain the services
            return services
                .AddSingleton<JsonService>()
                .AddSingleton<IEmailSenderService, EmailSenderService>(x => new EmailSenderService(configuration))
                .AddTransient<IAuthService, AuthService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IMemberService, MemberService>()
                .AddTransient<IAdminService, AdminService>()
                .AddTransient<IOwnerService, OwnerService>();
        }

        public static IServiceCollection RegisterFilters(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddMvc(opt => opt.Filters.Add(new RequestFilter(configuration))).Services;
        }
    }
}