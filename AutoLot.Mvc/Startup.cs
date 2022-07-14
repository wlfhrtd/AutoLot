using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoLot.Dal.EfStructures;
using AutoLot.Dal.Initialization;
using AutoLot.Dal.Repository;
using AutoLot.Dal.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoLot.Services.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AutoLot.Mvc.Models;


namespace AutoLot.Mvc
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
            services.AddControllersWithViews();
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
            // for custom tag helpers
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // WebOptimizer config
            if (_env.IsDevelopment() || _env.IsEnvironment("Local"))
            {
                services.AddWebOptimizer(false, false);
            }
            else
            {
                services.AddWebOptimizer(options =>
                {
                    options.MinifyCssFiles(); // ALL
                    // options.MinifyJsFiles(); // ALL
                    options.MinifyJsFiles("js/site.js"); // doesnt add "min" to names; minified versions are not on disk but cached
                    options.MinifyJsFiles("lib/**/*.js"); // compiler may complain on absence

                    options.AddJavaScriptBundle("js/validations/validationCode.js", "js/validations/**/*.js");
                    options.AddJavaScriptBundle(
                        "js/validations/validationCode.js",
                        "js/validations/validators.js",
                        "js/validations/errorFormatting.js"
                        );
                });
            }
            // DealerInfo
            services.Configure<DealerInfo>(Configuration.GetSection(nameof(DealerInfo)));
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
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts(); // HTTP Strict Transport Security Protocol
            }
            app.UseHttpsRedirection();
            // Libershark.WebOptimizer.Core, bundles and minifies assets
            app.UseWebOptimizer();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                /**
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                */
            });
        }
    }
}
