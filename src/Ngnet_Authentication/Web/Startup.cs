using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

using ApiModels.Common;
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
            MappingFactory.GenerateMapper(typeof(ClientErrorModel).GetTypeInfo().Assembly);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting()
                .UseCors(options => options
                      //.AllowAnyOrigin()
                      .WithOrigins(new string[4]
                      {
                          "http://localhost:3000",
                          "http://localhost:4200",
                          "http://localhost:5000",
                          "http://localhost:5001",
                      })
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
