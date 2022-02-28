using Context.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Diagnostics.CodeAnalysis;

namespace Context
{
    public class TasksContext : DbContext
    {
        public TasksContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(TasksContext).Assembly);
        }
    }

    public static class DbContextBuilderExtensions
    {
#if DEBUG
        private static ILoggerFactory _loggerFactory;
#endif
        /// <summary>
        /// Настроить контекст.
        /// </summary>
        /// <param name="optionsBuilder"><see cref="DbContextOptionsBuilder"/></param>
        /// <param name="configuration"><see cref="IConfiguration"/></param>
        public static DbContextOptionsBuilder SetOptions(this DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("context");
#if DEBUG
            _loggerFactory = LoggerFactory.Create(p => p.AddConsole());
#endif

            optionsBuilder.UseSqlServer(connectionString)
#if DEBUG
                        .UseLoggerFactory(_loggerFactory)
#endif
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors(true);

            return optionsBuilder;
        }
    }
    
}
