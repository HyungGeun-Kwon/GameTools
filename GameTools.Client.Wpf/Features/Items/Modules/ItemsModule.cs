using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Features.Items.Modules
{
    public static class ItemsModule
    {
        public static IServiceCollection AddItemsFeature(this IServiceCollection services)
        {
            services
                .AddItemsHostFeature()
                .AddItemsAuditsFeature()
                .AddItemsDataFeature();

            return services;
        }
    }
}
