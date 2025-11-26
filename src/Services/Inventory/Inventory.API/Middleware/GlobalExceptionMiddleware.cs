using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Inventory.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(exception, $"Ошибка. TraceId: {traceId}, Path: {context.Request.Path}, Method: {context.Request.Method}");

            var problemDetails = CreateProblemDetails(context, exception, traceId);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status ?? 500;

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception, string traceId)
        {
            var statusCode = exception switch
            {
                FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Type = "https://tools/ietf.org/html/rfc7807",
                Title = GetTitle(exception),
                Status = statusCode,
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] = traceId;

            if (exception is FluentValidation.ValidationException validationException)
            {
                var errors = validationException.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                problemDetails.Extensions["errors"] = errors;
            }

            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            return problemDetails;
        }

        private static string GetTitle(Exception exception)
        {
            return exception switch
            {
                FluentValidation.ValidationException => "Ошибка валидации",
                ArgumentException => "Неккоректный аргумент",
                InvalidOperationException => "Недопустимая операция",
                KeyNotFoundException => "Ресурс не найден",
                _ => "Внутренняя ошибка сервера"
            };
        }
    }
}
