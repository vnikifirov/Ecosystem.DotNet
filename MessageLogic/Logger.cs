using Microsoft.Extensions.Logging;
namespace MessageLogic
{
    public static class Logger
    {
        public static ILogger Create<T>()
        {
            using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
            var logger = loggerFactory.CreateLogger<T>();
            return logger;
        }
    }
}
