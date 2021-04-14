using AutoMapper;
using bank_identification_code.Persistence;
using bank_identification_code.Core;
using bank_identification_code.Core.Interfaces;
using bank_identification_code.Core.Models;
using bank_identification_code.Core.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using bank_identification_code.Persistence.Repositories;

namespace bank_identification_code
{
  public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //! DataBase Initializer works only if tables w/o records don't use in pordoction!
            services.AddTransient<DBInitializer>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //? Taking AppSettings Config
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //? Dependency Injections
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IReader, DBFFileReader>();

            services.AddScoped<IListEncoder, ListEncoder>();

            services.AddScoped<IEncoder, FromBase64Converter>();

            //? DI Repositories
            services.AddScoped<IRepository<BNKSEEKEntity>, BnkseekRepository>();
            services.AddScoped<IRepository<PZNEntity>, PZNRepository>();
            services.AddScoped<IRepository<REGEntity>, REGRepository>();
            services.AddScoped<IRepository<TNPEntity>, TNPRepository>();
            services.AddScoped<IRepository<UEREntity>, UERRepository>();

            //? Config AutoMapper
            services.AddAutoMapper();

            //? Connection string

            services.AddDbContext<BNKSEEKDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("server"));
                //! options.UseLazyLoadingProxies();
            });

            services.AddMvc();
        }

        // ? This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DBInitializer dBInitializer)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            //! DataBase Initializer works only if tables w/o records don't use in pordoction!
            dBInitializer.Seed().Wait();
        }
    }
}
