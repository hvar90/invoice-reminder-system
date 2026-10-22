using InvoicesApi.Models;
using MongoDB.Driver;

namespace InvoicesApi.Services;

public class CustomerService : ICustomerService
{
    private readonly IMongoCollection<Customer> _customersCollection;

    public CustomerService(IMongoDatabase database)
    {
        _customersCollection = database.GetCollection<Customer>("Customers");
    }

    public async Task<List<Customer>> GetAllAsync() =>
        await _customersCollection.Find(_ => true).ToListAsync();

    public async Task<Customer?> GetByIdAsync(string id) =>
        await _customersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
}