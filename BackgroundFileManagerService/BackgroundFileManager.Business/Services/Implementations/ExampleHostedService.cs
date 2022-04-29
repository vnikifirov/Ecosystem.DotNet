using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace BackgroundFile.Business.Implementations
{
    public class ExampleHostedService : IHostedService, IDisposable
    {
        private Timer _timer;

        public ExampleHostedService() { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state) => Console.WriteLine("Timed Background Service is working.");

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
