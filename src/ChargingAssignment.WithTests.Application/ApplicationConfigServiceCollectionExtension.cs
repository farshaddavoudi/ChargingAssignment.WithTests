using FluentValidation;
using CharginAssignment.WithTests.Application.Common.PipelineBehaviours;
using MediatR.NotificationPublishers;
using System.Reflection;

namespace CharginAssignment.WithTests.Application;

public static class ApplicationConfigServiceCollectionExtension
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationAssemblyEntryPoint).Assembly);

        // Fluent Validation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(configs =>
        {
            configs.RegisterServicesFromAssemblies(typeof(ApplicationAssemblyEntryPoint).Assembly);

            configs.Lifetime = ServiceLifetime.Scoped;

            configs.NotificationPublisher = new TaskWhenAllPublisher();

            // Pipelines order matters. MediatR executes them from top to bottom 
            configs.AddBehavior(typeof(IPipelineBehavior<,>), typeof(HasResponseValidationBehaviour<,>), ServiceLifetime.Scoped);
            configs.AddBehavior(typeof(IPipelineBehavior<,>), typeof(WithoutResponseValidationBehaviour<,>), ServiceLifetime.Scoped);
        });

        return services;
    }
}