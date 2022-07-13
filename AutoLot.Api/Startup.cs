using System;
using System.IO;
using System.Reflection;
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
using AutoLot.Services.Logging;
using AutoLot.Api.Filters;


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
            services
                .AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                    });
                })
                .AddControllers(config => config.Filters.Add(new CustomExceptionFilterAttribute(_env))) // filter at App level
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Pascal/camel case mess
                    options.JsonSerializerOptions.WriteIndented = true; // 'expanded'/tree view/notation
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true; // 400 auto response
                    options.SuppressInferBindingSourcesForParameters = true; // conventional inferring of values's sources for model binding (FromBody,FromForm,FromRoute,FromQuery)
                    options.SuppressConsumesConstraintForFormFileParameters = true; // FromForm part: 'auto' header multipart/form-data
                    options.SuppressMapClientErrors = true; // disable sending ProblemDetails when 404 happens
                });
            // db config
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
            // serilog
            services.AddScoped(typeof(IAppLogging<>), typeof(AppLogging<>));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "AutoLot Service",
                        Version = "v1",
                        Description = "Service to support AutoLot site",
                        License = new OpenApiLicense
                        {
                            Name = "strmbld wlfhrtd",
                            Url = new Uri("https://github.com/wlfhrtd"),
                        },
                    });

                // add xml documentation into swagger
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                // enable doc generation based on swashbuckle attributes for swagger
                c.EnableAnnotations();
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
            }
            // enable middleware to serve generated Swagger as endpoint
            app.UseSwagger();
            // enable middleware to serve swagger ui
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoLot Service v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
