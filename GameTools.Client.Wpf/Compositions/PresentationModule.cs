using GameTools.Client.Wpf.Shared.Components.Lookups.Rarities;
using GameTools.Client.Wpf.Shared.Components.Tabs;
using GameTools.Client.Wpf.Shared.Services.FilePickers;
using GameTools.Client.Wpf.Shell.ViewModels;
using GameTools.Client.Wpf.Shell.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Compositions
{
    public static class PresentationModule
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            // 전역 서비스
            services.AddTransient<IFilePickerService, FilePickerService>();

            // 공용 VM
            services.AddTransient<TabsHostViewModel>();
            services.AddSingleton<RarityLookupViewModel>();

            // Shell
            services.AddSingleton<MainViewModel>();
            services.AddTransient(sp => new MainWindow
            {
                DataContext = sp.GetRequiredService<MainViewModel>()
            });

            return services;
        }
    }
}
