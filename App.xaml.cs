using ImageTool.DBContexts;
using ImageTool.Extensions;
using ImageTool.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Hosting;
using System;
using System.Windows;

namespace ImageTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            this.host = CreateDefaultHost().Build();
        }

        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddSettings(configuration);

            services.AddHelpers();

            services.AddUploaders();

            services.AddEntityFrameworkMySql();
            services.AddEntityFrameworkProxies();

            services.AddDbContext<DBContext>((serviceProvider, optionBuilder) => optionBuilder
                .UseLazyLoadingProxies()
                .UseInternalServiceProvider(serviceProvider)
                .UseMySql(configuration.GetConnectionString("ImageDB"), ServerVersion.AutoDetect(configuration.GetConnectionString("ImageDB"))));

            services.AddBL();

            services.AddStores();

            services.AddManagements();

            services.AddViewModels();

            services.AddViews();

            services.AddWindows();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await host.StartAsync();

            host.Services.GetRequiredService<MainWindow>().Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (host)
            {
                await host.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }

        private IHostBuilder CreateDefaultHost() => Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
            .ConfigureLogging(logging => logging.ClearProviders())
            .UseNLog();
    }
}