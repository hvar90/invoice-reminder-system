using System.Net;
using System.Text.Json;

namespace FacturasApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, mensaje) = exception switch
        {
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
            ArgumentException => ((int)HttpStatusCode.BadRequest, exception.Message),
            // Muestra el mensaje detallado de la excepción en lugar del texto genérico
            _ => ((int)HttpStatusCode.InternalServerError, $"Error al procesar la solicitud: {exception.Message}")
        };

        context.Response.StatusCode = statusCode;

        var response = new { mensaje };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}