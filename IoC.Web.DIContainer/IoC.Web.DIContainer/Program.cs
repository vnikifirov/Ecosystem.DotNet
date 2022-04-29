using System.Threading.Tasks;
using IoC.Web.DIContainer.Business.Implementations;
using IoC.Web.DIContainer.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoC.Web.DIContainer.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new HostBuilder()
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService(sp => new Business.Implementations.BackgroundService(sp));

                services.AddTransient<IUnionService, UnionService>();

                services.AddSingleton<ISingletonService, OperationService>();
                services.AddScoped<IScopedService, OperationService>();
                services.AddTransient<ITransientService, OperationService>();
            })
            .RunConsoleAsync();
        }
    }
}
