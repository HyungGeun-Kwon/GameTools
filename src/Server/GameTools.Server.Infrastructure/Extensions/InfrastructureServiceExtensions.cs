using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Domain.Features.Items.Factories;
using GameTools.Server.Domain.Features.Items.Policies;
using GameTools.Server.Domain.Features.Items.Services;
using GameTools.Server.Domain.Features.Rarities.Factories;
using GameTools.Server.Domain.Features.Rarities.Policies;
using GameTools.Server.Domain.Features.Rarities.Services;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Auditing.Stores.ReadStores;
using GameTools.Server.Infrastructure.Persistence.Catalog.Checkers;
using GameTools.Server.Infrastructure.Persistence.Catalog.Seed;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.ReadStore;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore;
using GameTools.Server.Infrastructure.Persistence.Operations.Restores.Stores.ReadStores;
using GameTools.Server.Infrastructure.Persistence.Operations.Restores.Stores.WriteStores;
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

            services.AddScoped<IItemNameUniquenessChecker, ItemNameUniquenessChecker>();
            services.AddScoped<IItemNameUniquenessPolicy, ItemNameUniquenessPolicy>();
            services.AddScoped<IItemFactory, ItemFactory>();

            services.AddScoped<IRarityGradeUniquenessChecker, RarityGradeUniquenessChecker>();
            services.AddScoped<IRarityColorCodeUniquenessChecker, RarityColorCodeUniquenessChecker>();
            services.AddScoped<IRarityGradeUniquenessPolicy, RarityGradeUniquenessPolicy>();
            services.AddScoped<IRarityColorCodeUniquenessPolicy, RarityColorCodeUniquenessPolicy>();
            services.AddScoped<IRarityFactory, RarityFactory>();

            services.AddScoped<IItemWriteStore, ItemWriteStore>();
            services.AddScoped<IRarityWriteStore, RarityWriteStore>();
            services.AddScoped<IRestoreItemWriteStore, RestoreItemWriteStore>();

            services.AddScoped<IItemReadStore, ItemReadStore>();
            services.AddScoped<IRarityReadStore, RarityReadStore>();
            services.AddScoped<IItemAuditReadStore, ItemAuditReadStore>();
            services.AddScoped<IItemRestoreHistoryReadStore, ItemRestoreHistoryReadStore>();

            return services;
        }
    }
}
