using TODOList.Business.Context.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; 

namespace TODOList.BLL.Context.Context
{
    public class TodoListContext : DbContext
    {
        public TodoListContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Item> Todo { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(TodoListContext).Assembly);
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