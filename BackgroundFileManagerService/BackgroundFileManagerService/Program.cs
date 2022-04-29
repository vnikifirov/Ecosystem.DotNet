using BackgroundFile.Business.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackgroundFile.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ExampleHostedService>();
            })
            .RunConsoleAsync();
        }
    }
}
