using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using System.Configuration;
using System.Net.Http;
using System.Windows;
using VideoPlayerClient.Services.Interfaces;
using VideoPlayerClient.Services.VideoStreamerService;
using VideoPlayerClient.VideoStreamer;
using VideoPlayerClient.VideoStreamer.Interfaces;
using VideoPlayerClient.ViewModels;

namespace VideoPlayerClient
{
    public partial class App : Application
    {
        private static TimeSpan HttpHandlerLifetime = TimeSpan.FromMinutes(10);
        public static bool IsDesignMode { get; private set; } = true;
        protected override async void OnStartup(StartupEventArgs e)
        {
            IsDesignMode = false;
            var host = Host;
            base.OnStartup(e);
            await host.StartAsync().ConfigureAwait(false);
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            var host = Host;
            await host.StopAsync().ConfigureAwait(false);
            host.Dispose();
            _Host = null;
        }

        private static IHost? _Host;
        public static IHost Host => _Host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

        public static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            string? serverUrl = ConfigurationManager.AppSettings.Get("server_url");
            if (!IsDesignMode && serverUrl == null)
            {
                MessageBox.Show("Ошибка запуска приложения: параметр server_url не установлен.", "Ошибка");
                Environment.Exit(1);
            }
            //SetupHttpclient(services, serverUrl);
            services.AddSingleton<MainWindowViewModel>();
            services.AddScoped<IVideoStreamerService, VideoStreamerService>();
            services.AddScoped<IVideoStreamer, VideoStreamerHttp>();
        }
        //public static void SetupHttpclient(IServiceCollection services, string serverUrl)
        //{
        //    services.AddHttpClient<IPalindromeValidator, PalindromeValidatorFromServer>()
        //        .ConfigureHttpClient(client => client.BaseAddress = new Uri(serverUrl))
        //        .SetHandlerLifetime(HttpHandlerLifetime);
        //}
        
    }


}
