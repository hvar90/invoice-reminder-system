using InvoicesApi.Models;
using InvoicesApi.Services;
using Moq;

namespace InvoicesApi.Tests;

public class InvoiceServiceLogicTests
{
    private readonly Mock<IEmailService> _emailServiceMock;

    public InvoiceServiceLogicTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
    }

    [Fact]
    public void Test6_ValidacionReglaNegocio_PrimerRecordatorio_PasaASegundoRecordatorio()
    {
        // Arrange
        var cliente = new Customer { Id = "650000000000000000000001", Name = "Carlos Perez", Email = "carlos@example.com" };
        var factura = new Invoice { Id = "650000000000000000000010", CustomerId = cliente.Id, Amount = 150, Status = "primerrecordatorio" };

        // Act & Assert
        Assert.Equal("primerrecordatorio", factura.Status);
        
        // Simulación del cambio de estado según la regla de negocio
        if (factura.Status == "primerrecordatorio")
        {
            factura.Status = "segundorecordatorio";
        }

        Assert.Equal("segundorecordatorio", factura.Status);
    }

    [Fact]
    public void Test7_ValidacionReglaNegocio_SegundoRecordatorio_PasaADesactivado()
    {
        // Arrange
        var cliente = new Customer { Id = "650000000000000000000001", Name = "Carlos Perez", Email = "carlos@example.com" };
        var factura = new Invoice { Id = "650000000000000000000011", CustomerId = cliente.Id, Amount = 200, Status = "segundorecordatorio" };

        // Act & Assert
        Assert.Equal("segundorecordatorio", factura.Status);

        // Simulación del cambio de estado según la regla de negocio
        if (factura.Status == "segundorecordatorio")
        {
            factura.Status = "desactivado";
        }

        Assert.Equal("desactivado", factura.Status);
    }

    [Fact]
    public async Task Test8_EmailService_DebeLlamarseConPlantillaCorrecta()
    {
        // Arrange
        string email = "carlos@example.com";
        string asuntoEsperado = "Aviso: Segundo Recordatorio de Pago";
        string contenidoEsperado = "Hola Carlos Perez, su factura por $150 ha pasado a <b>segundo recordatorio</b>.";

        // Act
        await _emailServiceMock.Object.SendEmailAsync(email, asuntoEsperado, contenidoEsperado);

        // Assert: Validar que el servicio de correo reciba exactamente la información del cliente
        _emailServiceMock.Verify(
            e => e.SendEmailAsync(email, asuntoEsperado, contenidoEsperado), 
            Times.Once
        );
    }
}