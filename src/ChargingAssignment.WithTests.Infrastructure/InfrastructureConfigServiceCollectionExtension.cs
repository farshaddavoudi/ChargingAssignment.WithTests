using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Domain.AppConfigurationSettings;
using CharginAssignment.WithTests.Infrastructure.Persistence.EFCore;
using CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Repositories;
using CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace CharginAssignment.WithTests.Infrastructure;

public static class InfrastructureConfigServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddMemoryCache();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IChargeStationRepository, ChargeStationRepository>();

        // EFCore DbContext
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(appSettings.ConnStrSettings!.AppDbConnStr, sqlServerOptions =>
            {
                sqlServerOptions.CommandTimeout((int)TimeSpan.FromMinutes(1).TotalSeconds); //Default is 30 seconds
            });

            // Show Detailed Errors
            if (appSettings.IsDevelopment)
                options.EnableSensitiveDataLogging().EnableDetailedErrors();
        });

        return services;
    }
}