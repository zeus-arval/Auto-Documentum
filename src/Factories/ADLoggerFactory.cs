using AD.Factories.Common;
using Microsoft.Extensions.Logging;

namespace AD.Factories
{
    public class ADLoggerFactory : IFactory<ILogger>
    {
        public ILogger Create<T>()
        {
            return CreateLogger<T>();
        }

        private ILogger<T> CreateLogger<T>()
        {
            return LoggerFactory.Create(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Debug);

            }).CreateLogger<T>();
        }
    }
}