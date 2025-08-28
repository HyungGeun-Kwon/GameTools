using GameTools.Client.Application.Ports;
using GameTools.Client.Infrastructure.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GameTools.Client.Infrastructure.Extensions
{
    public sealed class ClientInfrastructureOptions
    {
        public Uri? BaseAddress { get; set; }
        public Func<IServiceProvider, Uri>? BaseAddressFactory { get; set; }
    }

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 게이트웨이/HttpClient + ActorHeaderHandler 등록.
        /// TActorProvider 구현을 Singleton으로 등록(이미 등록돼 있으면 건드리지 않음).
        /// </summary>
        public static IServiceCollection AddClientInfrastructure<TActorProvider>(
            this IServiceCollection services,
            Action<ClientInfrastructureOptions> configure)
            where TActorProvider : class, IActorProvider
        {
            var opt = new ClientInfrastructureOptions();
            configure(opt);

            // 이미 등록되어있다면 Pass
            services.TryAddSingleton<IActorProvider, TActorProvider>();

            services.AddTransient<ActorHeaderHandler>();

            Uri ResolveBase(IServiceProvider sp) =>
                opt.BaseAddress
                ?? opt.BaseAddressFactory?.Invoke(sp)
                ?? throw new InvalidOperationException("BaseAddress가 설정되지 않았습니다.");

            // Items
            services.AddHttpClient<IItemsGateway, ItemsGateway>((sp, c) => c.BaseAddress = ResolveBase(sp))
                .AddHttpMessageHandler<ActorHeaderHandler>();

            // Rarities
            services.AddHttpClient<IRarityGateway, RarityGateway>((sp, c) => c.BaseAddress = ResolveBase(sp))
                .AddHttpMessageHandler<ActorHeaderHandler>();

            return services;
        }

        /// <summary>
        /// 기본 ActorProvider(AppActorProvider)로 등록하는 간편 오버로드.
        /// </summary>
        public static IServiceCollection AddClientInfrastructure(
            this IServiceCollection services,
            Action<ClientInfrastructureOptions> configure)
            => AddClientInfrastructure<AppActorProvider>(services, configure);

        /// <summary>
        /// 문자열 URL 간편 오버로드.
        /// </summary>
        public static IServiceCollection AddClientInfrastructure<TActorProvider>(
            this IServiceCollection services, string baseUrl)
            where TActorProvider : class, IActorProvider
            => services.AddClientInfrastructure<TActorProvider>(o => o.BaseAddress = new Uri(baseUrl));

        /// <summary>
        /// 문자열 URL 간편 오버로드(기본 AppActorProvider).
        /// </summary>
        public static IServiceCollection AddClientInfrastructure(
            this IServiceCollection services, string baseUrl)
            => services.AddClientInfrastructure<AppActorProvider>(o => o.BaseAddress = new Uri(baseUrl));
    }
}
