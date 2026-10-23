using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using InvoicesApi.Controllers;
using InvoicesApi.Services;
using InvoicesApi.Models;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerService> _customerServiceMock;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _customerServiceMock = new Mock<ICustomerService>();
        _controller = new CustomersController(_customerServiceMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithListOfCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = "650000000000000000000001", Name = "Carlos Perez", Email = "carlos@example.com" }
        };
        _customerServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnCustomers = Assert.IsType<List<Customer>>(okResult.Value);
        Assert.Single(returnCustomers);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenCustomerExists()
    {
        // Arrange
        var customerId = "650000000000000000000001";
        var customer = new Customer { Id = customerId, Name = "Carlos Perez", Email = "carlos@example.com" };
        _customerServiceMock.Setup(s => s.GetByIdAsync(customerId)).ReturnsAsync(customer);

        // Act
        var result = await _controller.GetById(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnCustomer = Assert.IsType<Customer>(okResult.Value);
        Assert.Equal(customerId, returnCustomer.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = "invalid_id";
        _customerServiceMock.Setup(s => s.GetByIdAsync(customerId)).ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.GetById(customerId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}