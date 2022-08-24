using ApiModels.Common;
using Database;
using Mapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
            .AddDatabase(builder.Configuration)
            .AddDbContext<NgnetAuthDbContext>()
            .AddServices(builder.Configuration)
            .RegisterFilters(builder.Configuration)
            .AddControllers();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

MappingFactory.GenerateMapper(typeof(ClientErrorModel).GetTypeInfo().Assembly);

app
    .UseRouting()
    .UseCors(options => options
          .AllowAnyOrigin()
          //.WithOrigins(new string[4]
          //{
          //                "http://localhost:3000",
          //                "http://localhost:4200",
          //                "http://localhost:5000",
          //                "http://localhost:5001",
          //})
          .AllowAnyHeader()
          .AllowAnyMethod())
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    })
    .ApplyMigrations(builder.Configuration);

app.Run();