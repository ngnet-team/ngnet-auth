using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

using ApiModels;
using Database;
using Mapper;
using Web.Infrastructure;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDatabase(this.Configuration)
                .AddDbContext<NgnetAuthDbContext>()
                .AddServices(this.Configuration)
                .RegisterFilters(this.Configuration)
                .AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            MappingFactory.GenerateMapper(typeof(ErrorMessagesModel).GetTypeInfo().Assembly);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting()
                .UseCors(options => options
                      .AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod())
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .ApplyMigrations(this.Configuration);
        }
    }
}
