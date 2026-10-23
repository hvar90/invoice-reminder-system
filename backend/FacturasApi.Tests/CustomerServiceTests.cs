using Xunit;
using Moq;
using MongoDB.Driver;
using InvoicesApi.Services;
using InvoicesApi.Models;

public class CustomerServiceTests
{
    private readonly Mock<IMongoDatabase> _databaseMock;
    private readonly Mock<IMongoCollection<Customer>> _customersCollectionMock;
    private readonly Mock<IAsyncCursor<Customer>> _cursorMock;

    public CustomerServiceTests()
    {
        _databaseMock = new Mock<IMongoDatabase>();
        _customersCollectionMock = new Mock<IMongoCollection<Customer>>();
        _cursorMock = new Mock<IAsyncCursor<Customer>>();

        _databaseMock
            .Setup(db => db.GetCollection<Customer>("Customers", It.IsAny<MongoCollectionSettings>()))
            .Returns(_customersCollectionMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCustomersList()
    {
        // Arrange
        var customersList = new List<Customer>
        {
            new Customer { Id = "650000000000000000000001", Name = "Carlos Perez", Email = "carlos@example.com" }
        };

        _cursorMock.Setup(_ => _.Current).Returns(customersList);
        _cursorMock
            .SetupSequence(_ => _.MoveNextAsync(default))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _customersCollectionMock
            .Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Customer>>(), It.IsAny<FindOptions<Customer, Customer>>(), default))
            .ReturnsAsync(_cursorMock.Object);

        var customerService = new CustomerService(_databaseMock.Object);

        // Act
        var result = await customerService.GetAllAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}