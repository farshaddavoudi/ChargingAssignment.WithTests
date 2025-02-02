using CharginAssignment.WithTests.Domain.AppConfigurationSettings;
using CharginAssignment.WithTests.Domain.Constants;
using CharginAssignment.WithTests.Web.API.Middlewares;
using Microsoft.OpenApi.Models;

namespace CharginAssignment.WithTests.Web.API;

public static class ApiConfigServiceCollectionExtension
{
    public static IServiceCollection AddApiLayerServices(this IServiceCollection services, AppSettings appSettings)
    {
        // AppSettings as Singleton for easy use
        services.AddSingleton(sp => appSettings);

        #region Middlewares

        services.AddTransient<GlobalExceptionHandlingMiddleware>();

        services.AddTransient<AddMetadataToSerilogLogsMiddleware>();


        #endregion

        #region Swagger

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = AppMetadataConst.SolutionName,
                Description = $"Swagger for {AppMetadataConst.AppName}",
            });

            options.EnableAnnotations();
        });

        #endregion

        return services;
    }
}