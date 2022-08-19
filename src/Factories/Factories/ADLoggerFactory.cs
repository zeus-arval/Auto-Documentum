using AD.Aids.Common;
using Microsoft.Extensions.Logging;

namespace AD.Aids.Factories
{
    public class ADLoggerFactory : IFactory<ILogger>
    {
        private readonly bool _forTesting;

        public ADLoggerFactory(bool forTesting = false)
        {
            _forTesting = forTesting;
        }

        public ILogger Create<T>()
        {
            return CreateLogger<T>();
        }

        public ILogger CreateForTest<T>()
        {
            return CreateMuteLogger<T>();
        }

        private ILogger CreateLogger<T>()
        {
            if (_forTesting)
            {
                return CreateForTest<T>();
            }

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