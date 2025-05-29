using AD.Services.Factories;

namespace AD.Services.Common
{
    public class DIContainer
    {
        public IServiceProvider ServiceProvider { get; init; }
        public MainFactory MainFactory { get; init; }

        public DIContainer(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            MainFactory = new MainFactory(serviceProvider);
        }

    }
}
