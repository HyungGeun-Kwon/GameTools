using FluentValidation;
using GameTools.Server.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Server.Application.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
