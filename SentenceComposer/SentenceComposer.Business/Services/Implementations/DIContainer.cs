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
        private static DIContainer _instance { get; } = new DIContainer();
        public static DIContainer Instance
        {
            get => _instance;
        }

        private IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider != null)
                    return _serviceProvider;

                Init();

                return _serviceProvider;
            }
        }

        //private DIContainer() => Init();

        private DIContainer()
        {
            //setup our DI
            var serviceCollection = new ServiceCollection()
                //.AddScoped<ISentenceComposerService, SentenceComposerService>()
                .AddScoped<ITextReaderService, WebSiteTextReaderService>()
                //.AddScoped<ITextReaderService, CacheTextReaderService>()
                .AddScoped<IComposerService, ComposerService>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        
        private void Init()
        {
            //setup our DI
            var serviceCollection = new ServiceCollection()
                //.AddScoped<ISentenceComposerService, SentenceComposerService>()
                .AddScoped<ITextReaderService, WebSiteTextReaderService>()
                //.AddScoped<ITextReaderService, CacheTextReaderService>()
                .AddScoped<IComposerService, ComposerService>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
