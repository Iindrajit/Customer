using CustomerAPI.DbContexts;
using CustomerAPI.Services;
using CustomerAPI.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;

namespace CustomerAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(option =>
            {
                option.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CustomerAPI", Version = "v1" });
                c.DocumentFilter<JsonPatchDocumentFilter>();
            });

            services.AddDbContext<CustomerContext>(
                options => options.UseInMemoryDatabase(Configuration["Database"])
                );

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerAPI v1"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected error occured. Please try again later.");
                    });
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
