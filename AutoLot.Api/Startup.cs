using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using AutoLot.Dal.EfStructures;
using AutoLot.Dal.Initialization;
using AutoLot.Dal.Repository;
using AutoLot.Dal.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AutoLot.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            // possible to inject env and logger
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            string connectionString = Configuration.GetConnectionString("AutoLot");
            services.AddDbContextPool<ApplicationDbContext>(
                options => options.UseSqlServer(connectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure().CommandTimeout(60)));
            // ApplicationDbContext config
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<ICreditRiskRepository, CreditRiskRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IMakeRepository, MakeRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AutoLot.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // init dev db
                if (Configuration.GetValue<bool>("RebuildDataBase"))
                {
                    SampleDataInitializer.InitializeData(context);
                }

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoLot.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
