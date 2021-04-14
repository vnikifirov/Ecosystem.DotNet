namespace SwiftCode.Core.Persistence.Contexts
{
    using System.IO;
    using SwiftCode.Core.Properties;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.EntityFrameworkCore.Design;

    // ? Problem while adding new migration and update database
    // ? URL: https://codingblast.com/entityframework-core-idesigntimedbcontextfactory/
    public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BnkseekDbContext>
    {
        public BnkseekDbContext CreateDbContext(string[] args)
        {
            string root = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();
            string appsettingsPath = Path.Combine(root, Resources.appsettings ?? string.Empty);

            if (!File.Exists(appsettingsPath))
                throw new DirectoryNotFoundException("The appsettings.json file was not found.");    

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appsettingsPath)
                .Build();

            var builder = new DbContextOptionsBuilder<BnkseekDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new BnkseekDbContext(builder.Options);
        }
    }
}
