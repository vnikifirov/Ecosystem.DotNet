using Currency.Business.Models;
using Newtonsoft.Json;

namespace Currency.Business.Services.Implementations
{
    public class CurrencyService
    {
        private string _host;
        private string _path;

        public CurrencyService(string host, string path)
        {
            _host = host;
            _path = path;
        }
        public async Task<T> GetCurrencyAsync<T>(string currency) where T : Price
        {
            var urlBilder = new UriBuilder()
            {
                Scheme = "https",
                Host = _host,
                Path = _path + currency.ToLower()
            };
            var json = await Reader.ReadAsync(urlBilder.Uri);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
