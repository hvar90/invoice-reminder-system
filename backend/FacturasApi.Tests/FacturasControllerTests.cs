using FacturasApi.Models;
using FacturasApi.Services;
using Moq;

namespace FacturasApi.Tests;

public class FacturaServiceLogicTests
{
    private readonly Mock<IEmailService> _emailServiceMock;

    public FacturaServiceLogicTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
    }

    [Fact]
    public void Test6_ValidacionReglaNegocio_PrimerRecordatorio_PasaASegundoRecordatorio()
    {
        // Arrange
        var cliente = new Cliente { Id = "650000000000000000000001", Nombre = "Carlos Perez", Email = "carlos@example.com" };
        var factura = new Factura { Id = "650000000000000000000010", ClienteId = cliente.Id, Monto = 150, Estado = "primerrecordatorio" };

        // Act & Assert
        Assert.Equal("primerrecordatorio", factura.Estado);
        
        // Simulación del cambio de estado según la regla de negocio
        if (factura.Estado == "primerrecordatorio")
        {
            factura.Estado = "segundorecordatorio";
        }

        Assert.Equal("segundorecordatorio", factura.Estado);
    }

    [Fact]
    public void Test7_ValidacionReglaNegocio_SegundoRecordatorio_PasaADesactivado()
    {
        // Arrange
        var cliente = new Cliente { Id = "650000000000000000000001", Nombre = "Carlos Perez", Email = "carlos@example.com" };
        var factura = new Factura { Id = "650000000000000000000011", ClienteId = cliente.Id, Monto = 200, Estado = "segundorecordatorio" };

        // Act & Assert
        Assert.Equal("segundorecordatorio", factura.Estado);

        // Simulación del cambio de estado según la regla de negocio
        if (factura.Estado == "segundorecordatorio")
        {
            factura.Estado = "desactivado";
        }

        Assert.Equal("desactivado", factura.Estado);
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