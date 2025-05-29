using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AD.Services.Factories
{
    public class MainFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ADLoggerFactory _loggerFactory;

        public MainFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
            _loggerFactory = new ADLoggerFactory(loggerFactory);
        }

        public ILogger<T> CreateLogger<T>()
        {
            return (ILogger<T>)_loggerFactory.Create<T>();
        }
    }
}
