using Currency.Business.Services.Implementations;
using Currency.Business.Models;

namespace PriceMint
{
    public partial class Form1 : Form
    {
        private CurrencyService _currencyService;
        private System.Windows.Forms.Timer _timer;

        public Form1()
        {
            InitializeComponent();
            _currencyService = new CurrencyService("coincodex.com", "/api/coincodex/get_coin/");
            StartTimer();
        }

        private async Task DoWork()
        {
            SetNewGST((await _currencyService.GetCurrencyAsync<Gst>("gst2")).ToString());
            SetNewGMT((await _currencyService.GetCurrencyAsync<Gmt>("gmt")).ToString());
            SetNewSOL((await _currencyService.GetCurrencyAsync<Solana>("sol")).ToString());
        }

        public void StartTimer()
        {
            // _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000 * (int)TimeSpan.FromSeconds(5).TotalSeconds;
            _timer.Tick += async (sender, e) => await DoWork();
            _timer.Start();
        }

        private void SetNewGST(string currency)
        {
            // if the current thread isn't the UI thread
            if (gst.InvokeRequired)
            {
                // create a delegate for this method and push it to the UI thread
                //SetCountDelegate d = new SetCountDelegate(SetNewCounts);
                Action action = () => { gst.Text = currency; };
                Invoke(action);
            }
            else
            {
                // update the UI
                gst.Text = currency;
            }
        }

        private void SetNewGMT(string currency)
        {
            // if the current thread isn't the UI thread
            if (gmt.InvokeRequired)
            {
                // create a delegate for this method and push it to the UI thread
                //SetCountDelegate d = new SetCountDelegate(SetNewCounts);
                Action action = () => { gmt.Text = currency; };
                Invoke(action);
            }
            else
            {
                // update the UI
                gmt.Text = currency;
            }
        }

        private void SetNewSOL(string currency)
        {
            // if the current thread isn't the UI thread
            if (gmt.InvokeRequired)
            {
                // create a delegate for this method and push it to the UI thread
                //SetCountDelegate d = new SetCountDelegate(SetNewCounts);
                Action action = () => { Solana.Text = currency; };
                Invoke(action);
            }
            else
            {
                // update the UI
                Solana.Text = currency;
            }
        }
    }
}