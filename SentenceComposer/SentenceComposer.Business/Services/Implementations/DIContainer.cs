using System;
using Microsoft.Extensions.DependencyInjection;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Business.Services.Implementations
{
    public class DIContainer
    {
        /// <summary>
        /// Singleton of DIContainer
        /// </summary>
        /*private static DIContainer _instance { get; } = new DIContainer();
        public static DIContainer Instance
        {
            get => _instance;
        }*/

        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// Singleton of DI container
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider != null)
                    return _serviceProvider;

                Init();

                return _serviceProvider;
            }
        }

        private DIContainer() => Init();
        
        private static void Init()
        {
            //setup our DI
            var serviceCollection = new ServiceCollection()
                //.AddScoped<ISentenceComposerService, SentenceComposerService>()
                .AddScoped<ITextReaderService, HtmlAgilityPackTextReaderService>()
                //.AddScoped<ITextReaderService, CacheTextReaderService>()
                .AddScoped<IComposerService, ComposerService>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
