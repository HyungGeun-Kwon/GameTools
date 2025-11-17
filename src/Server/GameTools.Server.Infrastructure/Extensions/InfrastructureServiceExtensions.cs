using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Works;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Seed;
using GameTools.Server.Infrastructure.Persistence.Stores.ReadStore;
using GameTools.Server.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Server.Infrastructure.Persistence.Works;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Server.Infrastructure.Extensions
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
            services.AddScoped<IItemAuditReadStore, ItemAuditReadStore>();
            services.AddScoped<IRestoreRunReadStore, RestoreRunReadStore>();

            return services;
        }
    }
}
