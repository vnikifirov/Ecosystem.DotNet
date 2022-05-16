using AutoMapper;
using Business;
using Business.Configuration;
using Business.Mapping;
using Business.Services.Implementations;
using Business.Services.Interfaces;
using Context;
using Context.Repository.Implementations;
using Context.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestTaskTracker.Filters;

namespace TestTaskTracker
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
            var dbContextBuilder = new DbContextOptionsBuilder();
            services.AddDbContextPool<TasksContext>(opt => opt.SetOptions(Configuration));
            services.AddSingleton<Func<TasksContext>>(s => () => new TasksContext(dbContextBuilder.SetOptions(Configuration).Options));

            services.AddControllers();

            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Version = "v1" });

                c.ResolveConflictingActions(r => r.FirstOrDefault());

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            // Services and Repositories
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskService, TaskService>();

            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectService, ProjectService>();

            var configMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(typeof(TasksMapper));
                cfg.AddProfiles(typeof(ProejctsMapper));
            });

            IMapper mapper = configMapper.CreateMapper();
            //configMapper.AssertConfigurationIsValid();
            services.AddSingleton(mapper);

            services.Configure<TaskConfig>(Configuration.GetSection(nameof(TaskConfig)));
            services.AddMvc(x => x.Filters.Add<XPassFilter>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            //app.UseHttpsRedirection();

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }

        
    }
}
