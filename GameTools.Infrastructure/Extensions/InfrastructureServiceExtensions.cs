using GameTools.Application.Abstractions.Stores.ReadStore;
using GameTools.Application.Abstractions.Stores.WriteStore;
using GameTools.Application.Abstractions.Works;
using GameTools.Infrastructure.Persistence;
using GameTools.Infrastructure.Persistence.Seed;
using GameTools.Infrastructure.Persistence.Stores.ReadStore;
using GameTools.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Infrastructure.Persistence.Works;
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
            services.AddScoped<ISeeder, DataSeeder>();
            services.AddScoped<IItemWriteStore, ItemWriteStore>();
            services.AddScoped<IRarityWriteStore, RarityWriteStore>();
            services.AddScoped<IItemReadStore, ItemReadStore>();
            services.AddScoped<IRarityReadStore, RarityReadStore>();

            return services;
        }
    }
}
