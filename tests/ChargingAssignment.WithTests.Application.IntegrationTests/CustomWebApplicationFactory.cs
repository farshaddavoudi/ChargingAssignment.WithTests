using CharginAssignment.WithTests.Domain.AppConfigurationSettings;
using CharginAssignment.WithTests.Infrastructure.Persistence.EFCore;
using CharginAssignment.WithTests.Web.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CharginAssignment.WithTests.Application.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<ApiAssemblyEntryPoint>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextServiceDescriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextServiceDescriptor is not null)
            {
                services.Remove(dbContextServiceDescriptor);
            }

            var appSettings = GetAppSettings(builder, services);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("CharginAssignment_TestDB");
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });
        });

        base.ConfigureWebHost(builder);
    }

    #region Private Methods

    private static AppSettings GetAppSettings(IWebHostBuilder builder, IServiceCollection services)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddJsonFile("appsettings.json");
        });

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var appSettings = configuration.Get<AppSettings>();

        return appSettings!;
    }

    #endregion
}