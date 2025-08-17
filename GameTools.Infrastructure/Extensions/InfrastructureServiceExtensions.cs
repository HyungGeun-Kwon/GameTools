using GameTools.Application.Abstractions.ReadStore;
using GameTools.Application.Abstractions.Works;
using GameTools.Application.Abstractions.WriteStore;
using GameTools.Infrastructure.Persistence;
using GameTools.Infrastructure.Persistence.ReadStore;
using GameTools.Infrastructure.Persistence.Works;
using GameTools.Infrastructure.Persistence.WriteStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var cs = config.GetConnectionString("Default")
                ?? "Server=localhost;Database=GameDb;Trusted_Connection=True;TrustServerCertificate=True";

            services.AddDbContext<AppDbContext>((opt) 
                => opt.UseSqlServer(cs, sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IItemWriteStore, ItemWriteStore>();
            services.AddScoped<IRarityWriteStore, RarityWriteStore>();
            services.AddScoped<IItemReadStore, ItemReadStore>();
            services.AddScoped<IRarityReadStore, RarityReadStore>();

            return services;
        }
    }
}
