using FluentValidation;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;
using UnauthorizedAccessException = CharginAssignment.WithTests.Application.Common.Exceptions.UnauthorizedAccessException;

namespace CharginAssignment.WithTests.Web.API.Middlewares;

public class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            List<ErrorDetail> errorDetails = exception.Errors
                .Select(e => new ErrorDetail
                {
                    Field = e.PropertyName,
                    ErrorMessage = e.ErrorMessage
                }).ToList();

            var validationErrorCustomMessage = string.Join(" | ", errorDetails.Select(ed => ed.ErrorMessage));

            var error = new ErrorModel
            {
                Message = validationErrorCustomMessage,
                StatusCode = (int)HttpStatusCode.BadRequest,
                ErrorDetails = errorDetails,
#if DEBUG
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace
#endif
            };

            var exceptionResult = JsonSerializer.Serialize(error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(exceptionResult);

            logger.LogError(exception, "{exceptionResult}", exceptionResult);
        }
        catch (Exception ex)
        {
            // It will handle all exceptions thrown in the application
            // It is global, so it can be from DB error, service error or controller error 
            var exceptionResult = await HandleExceptionAsync(context, ex);

            logger.LogError(ex, "{exceptionResult}", exceptionResult);
        }
    }

    public async Task<string> HandleExceptionAsync(HttpContext context, Exception ex)
    {
        HttpStatusCode statusCode;
        string message;
        string? stackTrace;

        var exceptionType = ex.GetType();

        if (exceptionType == typeof(NotFoundException))
        {
            message = ex.Message;
            statusCode = HttpStatusCode.NotFound;
            stackTrace = ex.StackTrace;
        }
        else if (exceptionType == typeof(BadRequestException))
        {
            message = ex.Message;
            statusCode = HttpStatusCode.BadRequest;
            stackTrace = ex.StackTrace;
        }
        else if (exceptionType == typeof(GroupCapacityExceedsException))
        {
            message = ex.Message;
            statusCode = HttpStatusCode.UnprocessableEntity;
            stackTrace = ex.StackTrace;
        }
        else if (exceptionType == typeof(BusinessLogicException))
        {
            message = ex.Message;
            statusCode = HttpStatusCode.UnprocessableEntity;
            stackTrace = ex.StackTrace;
        }
        else if (exceptionType == typeof(UnauthorizedAccessException))
        {
            message = ex.Message;
            statusCode = HttpStatusCode.Unauthorized;
            stackTrace = ex.StackTrace;
        }
        else
        {
            message = "عملیات با خطا همراه شد"; //ex.Message;
            statusCode = HttpStatusCode.InternalServerError;
            stackTrace = ex.StackTrace;
        }

        var error = new ErrorModel
        {
            Message = message,
            StatusCode = (int)statusCode,
#if DEBUG
            ExceptionMessage = ex.Message,
            StackTrace = stackTrace
#endif
        };

        var exceptionResult = JsonSerializer.Serialize(error);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(exceptionResult);

        return exceptionResult;
    }
}

public class ErrorModel
{
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public List<ErrorDetail> ErrorDetails { get; set; } = new();

#if DEBUG
    public string? ExceptionMessage { get; set; }
    public string? StackTrace { get; set; }
#endif
}

public class ErrorDetail
{
    public string? Field { get; set; }
    public string? ErrorMessage { get; set; }
}