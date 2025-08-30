using System.Windows;
using System.Windows.Threading;
using DotNetHelper.MsDiKit.Extensions;
using GameTools.Client.Application.Extensions;
using GameTools.Client.Infrastructure.Extensions;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.ViewModels;
using GameTools.Client.Wpf.ViewModels.Items;
using GameTools.Client.Wpf.ViewModels.Navigations;
using GameTools.Client.Wpf.ViewModels.Rarities;
using GameTools.Client.Wpf.Views;
using GameTools.Client.Wpf.Views.Items;
using GameTools.Client.Wpf.Views.Navigations;
using GameTools.Client.Wpf.Views.Rarities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            // UI 스레드 예외
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // 백그라운드/스레드 예외
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 관찰되지 않은 Task 예외
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureServices(services =>
                {
                    var baseUri = "http://localhost:5008";

                    services.AddClientApplication();
                    services.AddClientInfrastructure(baseUri);

                    services.AddDialogService();
                    services.AddRegionService(() => _host.Services);

                    services.AddSingleton<MainViewModel>();

                    services.AddRegionView<EntityNavigationView, EntityNavigationViewModel>(RegionViewNames.Main_EntityNavigationView);

                    services.AddRegionView<ItemHostView, ItemHostViewModel>(RegionViewNames.Item_HostView);
                    services.AddRegionView<ItemSearchView, ItemSearchViewModel>(RegionViewNames.Item_SearchView);
                    services.AddRegionView<ItemResultView, ItemResultViewModel>(RegionViewNames.Item_ResultView);

                    services.AddRegionView<RarityHostView, RarityHostViewModel>(RegionViewNames.Rarity_HostView);
                    services.AddRegionView<RaritySearchView, RaritySearchViewModel>(RegionViewNames.Rarity_SearchView);
                    services.AddRegionView<RarityResultView, RarityResultViewModel>(RegionViewNames.Rarity_ResultView);
                }).Build();

            var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
            var mainWindow = new MainWindow() { DataContext = mainViewModel };

            MainWindow = mainWindow;
            MainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host is not null) await _host.StopAsync();
            _host?.Dispose();
            base.OnExit(e);
        }

        private void App_DispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                "Unhandled UI Exception\n\n" + e.Exception,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            try { Shutdown(-1); } catch { }
        }

        private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            var msg = e.ExceptionObject?.ToString() ?? "(no details)";
            MessageBox.Show(
                "Unhandled Non-UI Exception\n\n" + msg,
                "Fatal Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            try { Shutdown(-1); } catch { }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show(
                "Unobserved Task Exception\n\n" + e.Exception,
                "Task Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.SetObserved();
        }
    }
}
