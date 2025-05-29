using Microsoft.Extensions.Logging;

namespace AD.Services.Factories
{
    public class ADLoggerFactory(ILoggerFactory loggerFactory) : IFactory<ILogger>
    {
        private readonly ILoggerFactory _loggerFactory = loggerFactory;

        public ILogger Create<T>()
        {
            return CreateLogger<T>();
        }

        private ILogger CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }
    }
}