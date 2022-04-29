using static System.Console;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IoC.Web.DIContainer.Business.Implementations
{
    public class BackgroundService : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly IServiceProvider _serviceProvider;
        public BackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            WriteLine("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            WriteLine("Timed Background Service is working.");

            WriteLine("Begin Of Services SCOPE");

            using var scope = _serviceProvider.CreateScope();
            var _unionService = scope.ServiceProvider.GetService<IUnionService>() as UnionService;

            WriteLine("Hello from Config: " + _unionService._config["Key"]);

            WriteLine("transient - " + _unionService._transientService1.OperationID);
            WriteLine("transient - " + _unionService._transientService2.OperationID);
            WriteLine("singleton - " + _unionService._singletonService1.OperationID);
            WriteLine("singleton - " + _unionService._singletonService2.OperationID);
            WriteLine("scoped    - " + _unionService._scopedService1.OperationID);
            WriteLine("scoped    - " + _unionService._scopedService2.OperationID);

            WriteLine("End Of Services SCOPE");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteLine("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
