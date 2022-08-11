using Microsoft.Extensions.Logging;

namespace AD.Aids.Factories
{
    public class MainFactory
    {
        private readonly ADLoggerFactory _loggerFactory;

        public MainFactory()
        {
            _loggerFactory = new ADLoggerFactory();
        }

        public ILogger<T> CreateLogger<T>()
        {
            return (ILogger<T>)_loggerFactory.Create<T>();
        }

        public ILogger<T> CreateStubLogger<T>()
        {
            return (ILogger<T>)_loggerFactory.CreateForTest<T>();
        }
    }
}
