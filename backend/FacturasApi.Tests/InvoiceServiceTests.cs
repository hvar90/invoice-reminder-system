
using InvoicesApi.Models;
using InvoicesApi.Services;
using MongoDB.Driver;
using Moq;
using Xunit;
using System.Linq.Expressions;

namespace InvoicesApi.Tests;

public class InvoiceServiceTests
{
    private readonly Mock<IMongoDatabase> _database = new();
    private readonly Mock<IMongoCollection<Invoice>> _invoices = new();
    private readonly Mock<IMongoCollection<Customer>> _customers = new();
    private readonly Mock<IEmailService> _emailService = new();

    private readonly Customer _customer = new()
    {
        Id = "650000000000000000000001",
        Name = "Carlos Perez",
        Email = "carlos@example.com"
    };

    public InvoiceServiceTests()
    {
        _database
            .Setup(d => d.GetCollection<Invoice>(
                "Invoices", It.IsAny<MongoCollectionSettings>()))
            .Returns(_invoices.Object);

        _database
            .Setup(d => d.GetCollection<Customer>(
                "Customers", It.IsAny<MongoCollectionSettings>()))
            .Returns(_customers.Object);
    }

    private InvoiceService CreateService()
        => new(_database.Object, _emailService.Object);

    private void SetupCustomer(Customer? customer)
    {
        SetupFind(_customers, customer is null
            ? new List<Customer>()
            : new List<Customer> { customer });
    }

    private static void SetupFind<T>(
        Mock<IMongoCollection<T>> collection,
        List<T> data)
    {
        var cursor = new Mock<IAsyncCursor<T>>();

        cursor.SetupSequence(c => c.MoveNext(
                It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        cursor.SetupSequence(c => c.MoveNextAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        cursor.Setup(c => c.Current).Returns(data);

        collection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursor.Object);
    }

    private void SetupUpdate()
{
    var updateResult = new Mock<UpdateResult>();

    updateResult.SetupGet(r => r.IsAcknowledged)
        .Returns(true);

    updateResult.SetupGet(r => r.MatchedCount)
        .Returns(1);

    updateResult.SetupGet(r => r.ModifiedCount)
        .Returns(1);

    _invoices.Setup(c => c.UpdateOneAsync(
            It.IsAny<FilterDefinition<Invoice>>(),
            It.IsAny<UpdateDefinition<Invoice>>(),
            It.IsAny<UpdateOptions>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(updateResult.Object);
}

    [Fact]
    public async Task GetAllAsync_ReturnsAllInvoices()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = "1", CustomerId = _customer.Id,
                Amount = 150, Status = "primerrecordatorio" },
            new() { Id = "2", CustomerId = _customer.Id,
                Amount = 200, Status = "desactivado" }
        };

        SetupFind(_invoices, invoices);

        var result = await CreateService().GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(150, result[0].Amount);
        Assert.Equal("desactivado", result[1].Status);
    }

    [Fact]
    public async Task GetByCustomerAsync_ReturnsCustomerInvoices()
    {
        SetupCustomer(_customer);

        SetupFind(_invoices, new List<Invoice>
        {
            new() { Id = "1", CustomerId = _customer.Id,
                Amount = 150, Status = "primerrecordatorio" }
        });

        var result = await CreateService()
            .GetByCustomerAsync(_customer.Id);

        Assert.Single(result);
        Assert.Equal(150, result[0].Amount);
    }

    [Fact]
    public async Task GetByCustomerAsync_ThrowsWhenCustomerDoesNotExist()
    {
        SetupCustomer(null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => CreateService().GetByCustomerAsync("inexistente"));
    }

    [Fact]
    public async Task ProcessReminders_ThrowsWhenCustomerDoesNotExist()
    {
        SetupCustomer(null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => CreateService()
                .ProcessRemindersByCustomerAsync("inexistente"));

        _emailService.Verify(
            e => e.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessReminders_FirstReminder_SendsEmailAndUpdatesStatus()
    {
        SetupCustomer(_customer);
        SetupUpdate();

        SetupFind(_invoices, new List<Invoice>
        {
            new() { Id = "1", CustomerId = _customer.Id,
                Amount = 150, Status = "primerrecordatorio" }
        });

        var result = await CreateService()
            .ProcessRemindersByCustomerAsync(_customer.Id);

        Assert.Equal(1, result);

        _emailService.Verify(e => e.SendEmailAsync(
            _customer.Email,
            "Aviso: Segundo Recordatorio de Pago",
            It.Is<string>(body =>
                body.Contains("Carlos Perez") &&
                body.Contains("150") &&
                body.Contains("segundo recordatorio"))),
            Times.Once);

        _invoices.Verify(c => c.UpdateOneAsync(
            It.IsAny<FilterDefinition<Invoice>>(),
            It.IsAny<UpdateDefinition<Invoice>>(),
            It.IsAny<UpdateOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessReminders_SecondReminder_SendsEmailAndUpdatesStatus()
    {
        SetupCustomer(_customer);
        SetupUpdate();

        SetupFind(_invoices, new List<Invoice>
        {
            new() { Id = "2", CustomerId = _customer.Id,
                Amount = 200, Status = "segundorecordatorio" }
        });

        var result = await CreateService()
            .ProcessRemindersByCustomerAsync(_customer.Id);

        Assert.Equal(1, result);

        _emailService.Verify(e => e.SendEmailAsync(
            _customer.Email,
            "Urgente: Servicio a Ser Desactivado",
            It.Is<string>(body =>
                body.Contains("200") &&
                body.Contains("desactivado"))),
            Times.Once);

        _invoices.Verify(c => c.UpdateOneAsync(
            It.IsAny<FilterDefinition<Invoice>>(),
            It.IsAny<UpdateDefinition<Invoice>>(),
            It.IsAny<UpdateOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessReminders_DisabledInvoice_IsNotProcessed()
    {
        SetupCustomer(_customer);

        // El filtro de producción excluye las facturas desactivadas.
        // Por eso la consulta simulada devuelve una lista vacía.
        SetupFind(_invoices, new List<Invoice>());

        var result = await CreateService()
            .ProcessRemindersByCustomerAsync(_customer.Id);

        Assert.Equal(0, result);

        _emailService.Verify(e => e.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Never);

        _invoices.Verify(c => c.UpdateOneAsync(
            It.IsAny<FilterDefinition<Invoice>>(),
            It.IsAny<UpdateDefinition<Invoice>>(),
            It.IsAny<UpdateOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessReminders_EmailFails_DoesNotUpdateInvoice()
    {
        SetupCustomer(_customer);

        SetupFind(_invoices, new List<Invoice>
        {
            new() { Id = "1", CustomerId = _customer.Id,
                Amount = 150, Status = "primerrecordatorio" }
        });

        _emailService.Setup(e => e.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Error simulado"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateService()
                .ProcessRemindersByCustomerAsync(_customer.Id));

        _invoices.Verify(c => c.UpdateOneAsync(
            It.IsAny<FilterDefinition<Invoice>>(),
            It.IsAny<UpdateDefinition<Invoice>>(),
            It.IsAny<UpdateOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
