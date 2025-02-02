using CharginAssignment.WithTests.Domain.Constants;
using Serilog.Context;

namespace CharginAssignment.WithTests.Web.API.Middlewares;

public class AddMetadataToSerilogLogsMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using (LogContext.PushProperty("AppVersion", AppMetadataConst.AppVersion))
        {
            await next(context);
        }
    }
}