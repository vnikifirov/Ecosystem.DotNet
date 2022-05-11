using Newtonsoft.Json;
using System.Net;

namespace PriceMint
{
    public partial class Form1 : Form
    {
        private CurrencyService _currencyService;
        //private System.Timers.Timer _timer;
        private System.Windows.Forms.Timer _timer;
        object locker = new();
        public Form1()
        {
            InitializeComponent();
            _currencyService = new CurrencyService("coincodex.com", "/api/coincodex/get_coin/");
            StartTimer();
        }

        private async Task DoWork()
        {
            SetNewCounts((await _currencyService.GetCurrencyAsync<Gst>("gst2")).ToString());
            gmt.Text = (await _currencyService.GetCurrencyAsync<Gmt>("gmt")).ToString();
            Solana.Text = (await _currencyService.GetCurrencyAsync<Solana>("sol")).ToString();
        }
        public void StartTimer()
        {
            // _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            _timer = new System.Timers.Timer(5000);
            _timer.Elapsed += async (sender, e) => await DoWork();
            _timer.Start();
        }

        private void SetNewCounts(string currency)
        {
            // if the current thread isn't the UI thread
            if (gst.IsInvokeRequired)
            {
                // create a delegate for this method and push it to the UI thread
                SetCountDelegate d = new SetCountDelegate(SetNewCounts);
                this.Invoke(d, new object[] { currency });
            }
            else
            {
                // update the UI
                gst.Text = currency
            }
        }

    }
    public class CurrencyService
    {
        private string _host;
        private string _path;

        public CurrencyService(string host, string path)
        {
            _host = host;
            _path = path;
        }
        public async Task<T>  GetCurrencyAsync<T>(string currency) where T : Price
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
    public abstract class Price
    {
        [JsonProperty("last_price_usd")]
        public string LastPrice { get; set; }
        public override string ToString() => LastPrice.Substring(0,5);
    }
    public class Reader
    {
        public static async Task<string> ReadAsync(Uri url)
        {
            var web = new HttpClient();
            using var stream = await web.GetStreamAsync(url);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
    public sealed class Gst : Price { }
    public sealed class Gmt : Price { }
    public sealed class Solana : Price { }
}