using CharginAssignment.WithTests.Application;
using CharginAssignment.WithTests.Domain.AppConfigurationSettings;
using CharginAssignment.WithTests.Infrastructure;
using CharginAssignment.WithTests.Web.API;
using CharginAssignment.WithTests.Web.API.Extensions;
using CharginAssignment.WithTests.Web.API.Middlewares;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

// Configure Dependencies with Service Installers 
var appSettings = builder.Configuration.Get<AppSettings>();

appSettings!.IsDevelopment = builder.Environment.IsDevelopment();

builder.Services
    .AddApplicationLayerServices()
    .AddInfrastructureLayerServices(appSettings)
    .AddApiLayerServices(appSettings);

// Configure Serilog
builder.Host.ConfigureSerilog(appSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Specialized middleware(s) for development
}

app.UseMiddleware<AddMetadataToSerilogLogsMiddleware>();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseSwagger();

app.UseSwaggerUI(options => options.DocExpansion(DocExpansion.None));

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();