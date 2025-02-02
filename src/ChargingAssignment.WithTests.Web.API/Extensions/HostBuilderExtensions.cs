using CharginAssignment.WithTests.Domain.AppConfigurationSettings;
using CharginAssignment.WithTests.Domain.Constants;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

namespace CharginAssignment.WithTests.Web.API.Extensions;

public static class HostBuilderExtensions
{
    public static void ConfigureSerilog(this IHostBuilder hostBuilder, AppSettings appSettings)
    {
        hostBuilder.UseSerilog((builder, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Information()
                // Properties
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", AppMetadataConst.SolutionName)
                // Sinks
                .WriteTo.Console()
                .WriteTo.Debug(LogEventLevel.Error)
                .WriteTo.File("../../logs/log-.txt", rollingInterval: RollingInterval.Day)
                // Enrichers
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithClientIp()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers()
                    .WithDestructurers(new List<IExceptionDestructurer>
                    {
                new DbUpdateExceptionDestructurer(),
                    }));
        });

    }
}
