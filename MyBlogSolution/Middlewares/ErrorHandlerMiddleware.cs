using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using MyBlog.Common.Exceptions;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ErrorHandlerMiddleware> logger)
    {
        HttpStatusCode code;
        string result;

        switch (exception)
        {
            // FluentValidation hatalarını ele alıyoruz
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest; // 400 - Bad Request
                result = JsonSerializer.Serialize(new
                {
                    message = "Validation failed",
                    errors = validationException.Errors.Select(e => e.ErrorMessage).ToArray()
                });
                break;

            // Özel hataları (Custom Exceptions) ele alıyoruz
            case InvalidLoginException:
                code = HttpStatusCode.BadRequest; // 400 - Bad Request
                result = JsonSerializer.Serialize(new { message = exception.Message });
                break;

            // Diğer bilinmeyen hatalar
            default:
                code = HttpStatusCode.InternalServerError; // 500 - Internal Server Error
                result = JsonSerializer.Serialize(new { message = "An unexpected error occurred" });
                logger.LogError(exception, "Unhandled exception");
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}
