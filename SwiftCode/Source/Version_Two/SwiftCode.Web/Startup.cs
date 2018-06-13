using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwiftCode.Core.Extentions;
using SwiftCode.Core.Interfaces.Repositories;
using SwiftCode.Core.Interfaces.Services;
using SwiftCode.Core.Persistence.Contexts;
using SwiftCode.Core.Persistence.Repositories;
using SwiftCode.Core.Services;
using SwiftCode.Core.Utilities;

namespace SwiftCode.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //? Taking AppSettings Config
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //? Dependency Injections            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IReader, Reader>();
            services.AddScoped<IDecoder, Decoder>();
            services.AddScoped<IContextFactory>( 
                service => new ContextFactory(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IFileService, FileService>();

            //? DI Repositories
            services.AddScoped<IBnkseekRepository, BnkseekRepository>();
            services.AddScoped<IPznRepository, PznRepository>();
            services.AddScoped<IRegRepository, RegRepository>();
            services.AddScoped<ITnpRepository, TnpRepository>();
            services.AddScoped<IUerRepository, UerRepository>();

            //? Config AutoMapper
            services.AddAutoMapper();          

            //? Connection string            
            services.AddDbContext<BnkseekDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // Seed DataBase
            services.SeedDataBase();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
