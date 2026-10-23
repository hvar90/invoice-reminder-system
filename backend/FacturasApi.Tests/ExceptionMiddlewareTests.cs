using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using InvoicesApi.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InvoicesApi.Tests.Middlewares;

public class ExceptionMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;

    public ExceptionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var wasNextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            wasNextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(wasNextCalled);
        Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenKeyNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var exceptionMessage = "El recurso solicitado no existe";
        RequestDelegate next = (ctx) => throw new KeyNotFoundException(exceptionMessage);
        
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        Assert.Contains(exceptionMessage, responseBody);
    }

    [Fact]
    public async Task InvokeAsync_WhenArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var exceptionMessage = "El parámetro es inválido";
        RequestDelegate next = (ctx) => throw new ArgumentException(exceptionMessage);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        // Deserializar para validar el mensaje exacto sin importar el escape unicode
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody, jsonOptions);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        Assert.NotNull(responseObject);
        Assert.Equal(exceptionMessage, responseObject["message"]);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "Error en base de datos";
        RequestDelegate next = (ctx) => throw new InvalidOperationException(exceptionMessage);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        Assert.Contains($"Error al procesar la solicitud: {exceptionMessage}", responseBody);
    }
}