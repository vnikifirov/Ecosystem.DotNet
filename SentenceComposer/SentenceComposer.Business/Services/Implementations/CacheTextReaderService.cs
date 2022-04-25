using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Business.Services.Implementations
{
    public class CacheTextReaderService : ITextReaderService
    {
        private ITextReaderService _innerService { get; set; }
        private IMemoryCache _cacheService { get; set; }

        public CacheTextReaderService() => Init();

        private void Init()
        {
            var diContainer = DIContainer.Instance;
            _innerService = diContainer.ServiceProvider.GetService<ITextReaderService>();
            var options = GetMemoryCacheOptions();
            _cacheService = new MemoryCache(options);
        }

        public string ReadTextFrom(string source)
        {
            var key = $"url_{source}";
            if (!_cacheService.TryGetValue(key, out string text))
            {
                text = _innerService.ReadTextFrom(source);
                _cacheService.Set(key, text);
                return text;
            }

            return text;
        }

        public async Task<string> ReadTextFromAsync(string source)
        {
            var key = $"url_{source}";
            if (!_cacheService.TryGetValue(key, out string text))
            {
                text = await _innerService.ReadTextFromAsync(source);
                _cacheService.Set(key, text);
                return text;
            }

            return text;
        }

        private static MemoryCacheOptions GetMemoryCacheOptions()
        {
            var options = new MemoryCacheOptions();
            options.ExpirationScanFrequency = TimeSpan.FromDays(7);
            return options;
        }
    }
}
