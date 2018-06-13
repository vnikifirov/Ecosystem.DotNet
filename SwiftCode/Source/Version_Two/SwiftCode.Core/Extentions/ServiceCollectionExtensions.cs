
namespace SwiftCode.Core.Extentions
{
    using Microsoft.Extensions.DependencyInjection;
    using SwiftCode.Core.Interfaces.Services;
    using SwiftCode.Core.Services;

    public static class ServiceCollectionExtensions
    {
        public static void SeedDataBase(this IServiceCollection serviceCollection)        
        {
            serviceCollection.AddTransient<IFileService, FileService>();
            serviceCollection.AddTransient<IDecoder, Decoder>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
           
            //var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var fileService = serviceProvider.GetService<IFileService>();
            var decoder = serviceProvider.GetService<IDecoder>();

            Persistence.Contexts.SeedDataBase.Seed(fileService, decoder);
        }
    }
}
