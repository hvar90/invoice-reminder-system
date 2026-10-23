using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using InvoicesApi.Controllers;
using InvoicesApi.Services;
using InvoicesApi.Models;

public class InvoicesControllerTests
{
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly InvoicesController _controller;

    public InvoicesControllerTests()
    {
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _controller = new InvoicesController(_invoiceServiceMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithListOfInvoices()
    {
        // Arrange
        var mockList = new List<Invoice> { new Invoice { Id = "inv1", Amount = 150 } };
        _invoiceServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(mockList);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Invoice>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetByCustomer_ReturnsOkResult_WithCustomerInvoices()
    {
        // Arrange
        var customerId = "650000000000000000000001";
        var mockList = new List<Invoice> { new Invoice { Id = "inv1", CustomerId = customerId, Amount = 200 } };
        _invoiceServiceMock.Setup(s => s.GetByCustomerAsync(customerId)).ReturnsAsync(mockList);

        // Act
        var result = await _controller.GetByCustomer(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Invoice>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task ProcessRemindersByCustomer_ReturnsOkResult_WithProcessedCount()
    {
        // Arrange
        var customerId = "650000000000000000000001";
        _invoiceServiceMock.Setup(s => s.ProcessRemindersByCustomerAsync(customerId)).ReturnsAsync(2);

        // Act
        var result = await _controller.ProcessRemindersByCustomer(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}