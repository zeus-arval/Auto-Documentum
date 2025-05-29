using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting;
using Serilog.Extensions.Logging;
using Soft.Utils;
using System.Windows;
using Microsoft.Extensions.Logging;
using AD.Services.Common;
using Soft.MVVM;

namespace Soft
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        private IServiceProvider? _serviceProvider;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    var confBuilder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", false, true);
                    var configuration = confBuilder.Build();

                    RegisterServices(configuration, services);
                })
                .Build();
        }

        private void RegisterServices(IConfigurationRoot configuration, IServiceCollection services)
        {
            var loggingDir = configuration["Logging:LogDirectory"];
            var loggingFile = loggingDir!.TrimEnd('/') + "/AutoDocumentumLogs_.log";
            ITextFormatter formatter = new Serilog.Formatting.Display.MessageTemplateTextFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff zzz} {PaddedLevel} {SourceContextMin}: {Message}{NewLine}{Exception}", System.Globalization.CultureInfo.InvariantCulture);

            var serilogConf = new LoggerConfiguration()
                .Enrich.With<LogScopeEnricher>()
                .Enrich.With<PaddedLevelEnricher>()
                .Enrich.With<ScopeContextMinifierEnricher>()
                .MinimumLevel.Verbose()
                .WriteTo.Async(conf => conf
                    .File(
                        path: loggingFile,
                        formatter: formatter,
                        fileSizeLimitBytes: 10 * 1024 * 1024,
                        retainedFileCountLimit: null,
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true));
            var seriLogger = serilogConf.CreateLogger();

            services.AddLogging(builder =>
            {
                builder
                    .ClearProviders()
                    .AddConfiguration(configuration.GetSection("Logging"))
                    .AddSerilog(seriLogger, true)
                    .AddDebug();
            });
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DIContainer>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
            var startupForm = _serviceProvider!.GetRequiredService<MainWindow>();
            startupForm.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            AppHost!.Dispose();
            base.OnExit(e);
        }
    }
}
