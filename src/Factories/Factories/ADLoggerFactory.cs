using AD.Aids.Common;
using Microsoft.Extensions.Logging;

namespace AD.Aids.Factories
{
    public class ADLoggerFactory : IFactory<ILogger>
    {
        public ILogger Create<T>()
        {
            return CreateLogger<T>();
        }

        public ILogger CreateForTest<T>()
        {
            return CreateMuteLogger<T>();
        }

        private ILogger<T> CreateLogger<T>()
        {
            return LoggerFactory.Create(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Debug);


            }).CreateLogger<T>();
        }

        private ILogger<T> CreateMuteLogger<T>()
        {
            return LoggerFactory.Create(config =>
            {
                config.SetMinimumLevel(LogLevel.None);
            }).CreateLogger<T>();
        }
    }
}