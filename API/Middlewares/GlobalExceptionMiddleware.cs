using Application.Common.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace API.Middlewares;

/// <summary>
/// Middleware لمعالجة الأخطاء العامة
/// يمنع ظهور تفاصيل الخطأ للمستخدم ويُرجع رسالة واضحة
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ غير متوقع: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException notFound => (HttpStatusCode.NotFound, notFound.Message),
            ValidationException validation => (HttpStatusCode.BadRequest,
                string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))),
            _ => (HttpStatusCode.InternalServerError, "حدث خطأ داخلي في الخادم")
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(response);
    }
}
