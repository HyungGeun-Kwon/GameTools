using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using CsvHelper;
using DotNetHelper.MsDiKit.Extensions;
using GameTools.Client.Application.Extensions;
using GameTools.Client.Infrastructure.Extensions;
using GameTools.Client.Wpf.Compositions;
using GameTools.Client.Wpf.Features.Items.Modules;
using GameTools.Client.Wpf.Features.Navigations.Modules;
using GameTools.Client.Wpf.Features.Rarities.Modules;
using GameTools.Client.Wpf.Features.RestoreHistories.Modules;
using GameTools.Client.Wpf.Shared.UI.Behaviors;
using GameTools.Client.Wpf.Shell.ViewModels;
using GameTools.Client.Wpf.Shell.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GameTools.Client.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IHost _host = null!;
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Async(a => a.File(
                    path: "logs/log-.log", // logs/log-YYYY-MM-DD.log
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 365
                )).CreateLogger();

            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder(e.Args)
                .UseSerilog()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    var baseUri = context.Configuration.GetValue<string>("Api:BaseUri")
                        ?? throw new InvalidOperationException("Config 'Api:BaseUri' is missing.");
                    services.AddClientApplication();
                    services.AddClientInfrastructure(baseUri);

                    services.AddPresentation();

                    services.AddDialogService();
                    services.AddRegionService(() => _host.Services);

                    services.AddItemsFeature();
                    services.AddRaritiesFeature();
                    services.AddNavigationsFeature();
                    services.AddRestoreHistoriesFeature();

                }).Build();

            _host.StartAsync().GetAwaiter().GetResult();

            TabRegionAttached.Configure(() => _host.Services);

            var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
            var mainWindow = new MainWindow() { DataContext = mainViewModel };
            MainWindow = mainWindow;
            MainWindow.Show();

            Log.Information("Application started");
        }

        protected override async void OnExit(ExitEventArgs e)
        {

            try
            {
                if (_host is not null) await _host.StopAsync();
                _host?.Dispose();
            }
            finally
            {
                Log.CloseAndFlush();
                base.OnExit(e);
            }
        }

        private void App_DispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (IsCancellation(e.Exception))
            {
                e.Handled = true; return;
            }

            if (IsNormalError(e.Exception))
            {
                e.Handled = true;
                Log.Error(e.Exception, "UI thread exception");
                MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Log.Fatal(e.Exception, "UI thread unhandled exception");
        }

        private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            var terminating = e.IsTerminating;

            var title = e.IsTerminating ? "Fatal Error" : "Unhandled Error";

            Log.Fatal(ex, $"Unhandled exception. IsTerminating={terminating}");

            try
            {
                var msg = $"Unhandled Exception (IsTerminating={terminating})\n\n{ex}";
                if (Current?.Dispatcher is Dispatcher d)
                {
                    d.BeginInvoke(() =>
                        MessageBox.Show(msg, terminating ? "Fatal Error" : "Unhandled Error",
                            MessageBoxButton.OK, MessageBoxImage.Error));
                }
                else
                {
                    MessageBox.Show(msg, terminating ? "Fatal Error" : "Unhandled Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }

            try { Log.CloseAndFlush(); } catch { }
            if (!e.IsTerminating) try { Shutdown(-1); } catch { }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            var agg = e.Exception.Flatten();

            if (agg.InnerExceptions.All(IsCancellation))
            {
                e.SetObserved();
                return;
            }

            Log.Error(agg, "Unobserved task exception");

            Dispatcher.BeginInvoke(() =>
                MessageBox.Show("An error occurred while running a background task.\n\nPlease check the log for more details.",
                    "Task Error", MessageBoxButton.OK, MessageBoxImage.Error));

            e.SetObserved();
        }

        private static bool IsCancellation(Exception? ex)
        {
            if (ex is null) return false;
            if (ex is OperationCanceledException || ex is TaskCanceledException) return true;
            if (ex is AggregateException ae) return ae.Flatten().InnerExceptions.All(IsCancellation);
            return ex.InnerException is not null && IsCancellation(ex.InnerException);
        }

        private static bool IsNormalError(Exception? ex)
        {
            return ex is HttpRequestException
                || ex is InvalidOperationException
                || ex is TaskCanceledException
                || ex is JsonException
                || ex is AuthenticationException
                || ex is ArgumentNullException
                || ex is IOException
                || ex is CsvHelperException;
        }
    }
}
